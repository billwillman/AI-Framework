# 摄像头 Hook 与 AI 图像处理方案（换衣服）

## 目录

1. [整体架构](#1-整体架构)
2. [Android 方案](#2-android-方案)
3. [iOS 方案](#3-ios-方案)
4. [AI 换衣 Pipeline](#4-ai-换衣-pipeline)
5. [核心代码示例](#5-核心代码示例)
6. [方案对比](#6-方案对比)
7. [开源项目参考](#7-开源项目参考)
8. [注意事项](#8-注意事项)

---

## 1. 整体架构

```
真实摄像头 → Hook/拦截层 → AI图像处理（换衣） → 虚拟摄像头 → 第三方App
```

**核心思路**：拦截摄像头帧数据 → 经过 AI 模型处理（如换衣服）→ 将处理后的帧数据注入回摄像头数据流，使第三方应用读取到的是处理后的画面。

---

## 2. Android 方案

### 2.1 方案一：Xposed/LSPosed Hook（需 Root）

**原理**：通过 Xposed 框架 Hook `Camera2 API`，在回调中拦截并替换图像帧。

**Hook 目标**：
- `CameraCaptureSession.StateCallback`
- `ImageReader.acquireLatestImage()`
- `CameraCaptureSession.CaptureCallback`

**代码示例（Java）**：

```java
public class CameraHookModule implements IXposedHookLoadPackage {
    
    @Override
    public void handleLoadPackage(XC_LoadPackage.LoadPackageParam lpparam) {
        // Hook ImageReader.acquireLatestImage()
        XposedHelpers.findAndHookMethod(
            "android.media.ImageReader",
            lpparam.classLoader,
            "acquireLatestImage",
            new XC_MethodHook() {
                @Override
                protected void afterHookedMethod(MethodHookParam param) throws Throwable {
                    Image image = (Image) param.getResult();
                    if (image != null) {
                        // 获取图像数据
                        Image.Plane[] planes = image.getPlanes();
                        ByteBuffer buffer = planes[0].getBuffer();
                        
                        // 发送到AI处理模块
                        byte[] processedData = AIProcessor.process(buffer);
                        
                        // 替换原始数据
                        buffer.clear();
                        buffer.put(processedData);
                    }
                }
            }
        );
        
        // Hook Camera2 CaptureCallback
        XposedHelpers.findAndHookMethod(
            "android.hardware.camera2.CameraCaptureSession$CaptureCallback",
            lpparam.classLoader,
            "onCaptureCompleted",
            "android.hardware.camera2.CameraCaptureSession",
            "android.hardware.camera2.CaptureRequest",
            "android.hardware.camera2.TotalCaptureResult",
            new XC_MethodHook() {
                @Override
                protected void beforeHookedMethod(MethodHookParam param) throws Throwable {
                    // 在回调触发前修改帧数据
                }
            }
        );
    }
}
```

### 2.2 方案二：虚拟摄像头（Android 14+，无需 Root）

**原理**：通过 `VirtualDeviceManager` + `VirtualCameraConfig` 注册虚拟摄像头设备。

**代码示例（Kotlin）**：

```kotlin
// Android 14+ VirtualDeviceManager
val virtualDeviceManager = getSystemService(Context.VIRTUAL_DEVICE_SERVICE) as VirtualDeviceManager

val cameraConfig = VirtualCameraConfig.Builder("virtual_ai_camera")
    .setLensFacing(CameraCharacteristics.LENS_FACING_FRONT)
    .addStreamConfig(1920, 1080, ImageFormat.YUV_420_888, 30)
    .build()

val virtualCamera = virtualDeviceManager.createVirtualCamera(cameraConfig)

// 持续写入处理后的帧
fun feedProcessedFrame(processedBitmap: Bitmap) {
    val surface = virtualCamera.surface
    val canvas = surface.lockCanvas(null)
    canvas.drawBitmap(processedBitmap, 0f, 0f, null)
    surface.unlockCanvasAndPost(canvas)
}
```

### 2.3 方案三：Magisk 模块 + V4L2 Loopback

**原理**：加载自定义内核模块，创建虚拟 `/dev/video*` 设备节点。

**步骤**：
1. 编译 V4L2 loopback 内核模块
2. 通过 Magisk 模块在启动时加载
3. 创建虚拟 `/dev/video*` 设备
4. 用户空间程序读取真实摄像头 → AI 处理 → 写入虚拟设备
5. 第三方 App 打开虚拟摄像头设备

---

## 3. iOS 方案

### 3.1 方案一：越狱 + CydiaSubstrate/Theos

**原理**：Hook `AVCaptureVideoDataOutput` 的 `setSampleBufferDelegate:queue:`，用 ProxyDelegate 拦截 `CMSampleBufferRef`，修改 `CVPixelBufferRef` 像素数据。

**代码示例（Objective-C）**：

```objc
// Theos Tweak
#import <AVFoundation/AVFoundation.h>

@interface CameraProxy : NSObject <AVCaptureVideoDataOutputSampleBufferDelegate>
@property (nonatomic, weak) id<AVCaptureVideoDataOutputSampleBufferDelegate> originalDelegate;
@end

@implementation CameraProxy

- (void)captureOutput:(AVCaptureOutput *)output 
didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer 
       fromConnection:(AVCaptureConnection *)connection {
    
    // 获取像素缓冲区
    CVPixelBufferRef pixelBuffer = CMSampleBufferGetImageBuffer(sampleBuffer);
    CVPixelBufferLockBaseAddress(pixelBuffer, 0);
    
    void *baseAddress = CVPixelBufferGetBaseAddress(pixelBuffer);
    size_t width = CVPixelBufferGetWidth(pixelBuffer);
    size_t height = CVPixelBufferGetHeight(pixelBuffer);
    size_t bytesPerRow = CVPixelBufferGetBytesPerRow(pixelBuffer);
    
    // AI 处理：换衣服
    [AIClothProcessor processPixelData:baseAddress 
                                 width:width 
                                height:height 
                           bytesPerRow:bytesPerRow];
    
    CVPixelBufferUnlockBaseAddress(pixelBuffer, 0);
    
    // 将修改后的数据传递给原始delegate
    [self.originalDelegate captureOutput:output 
                   didOutputSampleBuffer:sampleBuffer 
                          fromConnection:connection];
}

@end

// Hook AVCaptureVideoDataOutput
%hook AVCaptureVideoDataOutput

- (void)setSampleBufferDelegate:(id<AVCaptureVideoDataOutputSampleBufferDelegate>)delegate 
                          queue:(dispatch_queue_t)queue {
    CameraProxy *proxy = [[CameraProxy alloc] init];
    proxy.originalDelegate = delegate;
    %orig(proxy, queue);
}

%end
```

### 3.2 方案二：Frida 动态注入

**原理**：通过 Frida JS 脚本 Hook `AVCaptureVideoDataOutput` 和 `CMSampleBufferGetImageBuffer`。

**代码示例**：

```python
# frida_camera_hook.py
import frida

js_code = """
// Hook AVCaptureVideoDataOutput
var AVCaptureVideoDataOutput = ObjC.classes.AVCaptureVideoDataOutput;

Interceptor.attach(
    AVCaptureVideoDataOutput['- setSampleBufferDelegate:queue:'].implementation, 
    {
        onEnter: function(args) {
            console.log('[*] Camera delegate being set');
            // 替换 delegate
            var originalDelegate = ObjC.Object(args[2]);
            // 创建代理对象拦截帧数据
        }
    }
);

// Hook CMSampleBufferGetImageBuffer
var CMSampleBufferGetImageBuffer = Module.findExportByName(
    'CoreMedia', 'CMSampleBufferGetImageBuffer'
);

Interceptor.attach(CMSampleBufferGetImageBuffer, {
    onLeave: function(retval) {
        // retval 是 CVPixelBufferRef
        // 在这里修改像素数据
        var pixelBuffer = retval;
        // 处理帧数据...
    }
});
"""

device = frida.get_usb_device()
session = device.attach("目标App")
script = session.create_script(js_code)
script.load()
```

---

## 4. AI 换衣 Pipeline

### 4.1 技术选型

| 推理框架 | 平台 | 特点 |
|---------|------|------|
| **ncnn** | Android/iOS | 腾讯开源，移动端优化最佳 |
| **ONNX Runtime Mobile** | Android/iOS | 微软开源，模型兼容性好 |
| **TFLite** | Android/iOS | Google 官方，生态完善 |
| **MediaPipe** | Android/iOS | Google，端到端解决方案 |
| **MNN** | Android/iOS | 阿里开源，性能优秀 |

### 4.2 处理流程

```
输入帧 → 人体检测(YOLOv8-Pose) → 人体分割(SegFormer/MediaPipe) 
       → 姿态估计(关键点) → 衣服区域提取 
       → 虚拟试衣(Warping + GAN) → Alpha Blending 融合 → 输出帧
```

**详细步骤**：

1. **人体检测**：使用 YOLOv8-Pose 检测人体并获取关键点
2. **人体分割**：使用 SegFormer 或 MediaPipe Selfie Segmentation 获取人体 Mask
3. **姿态估计**：提取 18/25 个身体关键点（肩、肘、腕、髋等）
4. **衣服区域提取**：根据关键点和 Mask 确定上衣/裤子区域
5. **虚拟试衣**：
   - **Warping 阶段**：根据姿态将目标衣服图像变形（TPS 变换）
   - **GAN 阶段**：生成自然的穿着效果（融合光照、褶皱）
6. **Alpha Blending 融合**：将处理后的衣服区域与原始帧混合

---

## 5. 核心代码示例

### 5.1 C++ ncnn 推理（换衣处理器）

```cpp
#include <ncnn/net.h>
#include <ncnn/mat.h>
#include <opencv2/opencv.hpp>

class VirtualTryOn {
private:
    ncnn::Net segmentNet;    // 人体分割模型
    ncnn::Net poseNet;       // 姿态估计模型
    ncnn::Net tryonNet;      // 换衣模型
    cv::Mat targetCloth;     // 目标衣服图像
    
public:
    bool init(const std::string& modelDir) {
        // 加载分割模型
        segmentNet.load_param((modelDir + "/segment.param").c_str());
        segmentNet.load_model((modelDir + "/segment.bin").c_str());
        
        // 加载姿态估计模型
        poseNet.load_param((modelDir + "/pose.param").c_str());
        poseNet.load_model((modelDir + "/pose.bin").c_str());
        
        // 加载换衣模型
        tryonNet.load_param((modelDir + "/tryon.param").c_str());
        tryonNet.load_model((modelDir + "/tryon.bin").c_str());
        
        return true;
    }
    
    void setTargetCloth(const cv::Mat& cloth) {
        targetCloth = cloth.clone();
    }
    
    // 人体分割
    cv::Mat segmentBody(const cv::Mat& frame) {
        ncnn::Mat input = ncnn::Mat::from_pixels_resize(
            frame.data, ncnn::Mat::PIXEL_BGR2RGB,
            frame.cols, frame.rows, 256, 256
        );
        
        // 归一化
        const float mean_vals[3] = {0.485f * 255.f, 0.456f * 255.f, 0.406f * 255.f};
        const float norm_vals[3] = {1.0f / (0.229f * 255.f), 1.0f / (0.224f * 255.f), 1.0f / (0.225f * 255.f)};
        input.substract_mean_normalize(mean_vals, norm_vals);
        
        ncnn::Extractor ex = segmentNet.create_extractor();
        ex.input("input", input);
        
        ncnn::Mat output;
        ex.extract("output", output);
        
        // 转换为 OpenCV Mat (mask)
        cv::Mat mask(output.h, output.w, CV_32FC1, output.data);
        cv::Mat maskResized;
        cv::resize(mask, maskResized, frame.size());
        
        cv::Mat binaryMask;
        cv::threshold(maskResized, binaryMask, 0.5, 1.0, cv::THRESH_BINARY);
        binaryMask.convertTo(binaryMask, CV_8UC1, 255);
        
        return binaryMask;
    }
    
    // 姿态估计
    std::vector<cv::Point2f> estimatePose(const cv::Mat& frame) {
        ncnn::Mat input = ncnn::Mat::from_pixels_resize(
            frame.data, ncnn::Mat::PIXEL_BGR2RGB,
            frame.cols, frame.rows, 192, 256
        );
        
        const float mean_vals[3] = {127.5f, 127.5f, 127.5f};
        const float norm_vals[3] = {1.0f / 127.5f, 1.0f / 127.5f, 1.0f / 127.5f};
        input.substract_mean_normalize(mean_vals, norm_vals);
        
        ncnn::Extractor ex = poseNet.create_extractor();
        ex.input("input", input);
        
        ncnn::Mat heatmaps;
        ex.extract("heatmaps", heatmaps);
        
        std::vector<cv::Point2f> keypoints;
        int numKeypoints = heatmaps.c;
        
        for (int i = 0; i < numKeypoints; i++) {
            const float* heatmap = heatmaps.channel(i);
            int maxIdx = 0;
            float maxVal = heatmap[0];
            
            for (int j = 1; j < heatmaps.h * heatmaps.w; j++) {
                if (heatmap[j] > maxVal) {
                    maxVal = heatmap[j];
                    maxIdx = j;
                }
            }
            
            float x = (maxIdx % heatmaps.w) * frame.cols / (float)heatmaps.w;
            float y = (maxIdx / heatmaps.w) * frame.rows / (float)heatmaps.h;
            keypoints.push_back(cv::Point2f(x, y));
        }
        
        return keypoints;
    }
    
    // 虚拟试衣处理
    cv::Mat tryOn(const cv::Mat& frame, const cv::Mat& mask, 
                  const std::vector<cv::Point2f>& keypoints) {
        // 准备输入
        ncnn::Mat inputFrame = ncnn::Mat::from_pixels_resize(
            frame.data, ncnn::Mat::PIXEL_BGR2RGB,
            frame.cols, frame.rows, 256, 256
        );
        
        ncnn::Mat inputCloth = ncnn::Mat::from_pixels_resize(
            targetCloth.data, ncnn::Mat::PIXEL_BGR2RGB,
            targetCloth.cols, targetCloth.rows, 256, 256
        );
        
        ncnn::Mat inputMask = ncnn::Mat::from_pixels_resize(
            mask.data, ncnn::Mat::PIXEL_GRAY,
            mask.cols, mask.rows, 256, 256
        );
        
        ncnn::Extractor ex = tryonNet.create_extractor();
        ex.input("person", inputFrame);
        ex.input("cloth", inputCloth);
        ex.input("mask", inputMask);
        
        ncnn::Mat output;
        ex.extract("output", output);
        
        // 转回 OpenCV Mat
        cv::Mat result(256, 256, CV_8UC3);
        output.to_pixels(result.data, ncnn::Mat::PIXEL_RGB2BGR);
        cv::resize(result, result, frame.size());
        
        return result;
    }
    
    // 主处理函数：处理每一帧
    cv::Mat processFrame(const cv::Mat& frame) {
        if (targetCloth.empty()) return frame;
        
        // Step 1: 人体分割
        cv::Mat mask = segmentBody(frame);
        
        // Step 2: 姿态估计
        std::vector<cv::Point2f> keypoints = estimatePose(frame);
        
        // Step 3: 虚拟试衣
        cv::Mat result = tryOn(frame, mask, keypoints);
        
        // Step 4: Alpha Blending 融合
        cv::Mat output = frame.clone();
        cv::Mat maskFloat;
        mask.convertTo(maskFloat, CV_32FC1, 1.0 / 255.0);
        
        // 对 mask 进行羽化处理
        cv::GaussianBlur(maskFloat, maskFloat, cv::Size(5, 5), 2.0);
        
        for (int y = 0; y < output.rows; y++) {
            for (int x = 0; x < output.cols; x++) {
                float alpha = maskFloat.at<float>(y, x);
                cv::Vec3b& dst = output.at<cv::Vec3b>(y, x);
                cv::Vec3b src = result.at<cv::Vec3b>(y, x);
                
                dst[0] = (uchar)(alpha * src[0] + (1.0f - alpha) * dst[0]);
                dst[1] = (uchar)(alpha * src[1] + (1.0f - alpha) * dst[1]);
                dst[2] = (uchar)(alpha * src[2] + (1.0f - alpha) * dst[2]);
            }
        }
        
        return output;
    }
};
```

### 5.2 Android JNI 桥接

```java
public class NativeProcessor {
    static {
        System.loadLibrary("virtual_tryon");
    }
    
    public static native boolean init(String modelDir);
    public static native void setTargetCloth(Bitmap cloth);
    public static native Bitmap processFrame(Bitmap frame);
    public static native void release();
}
```

---

## 6. 方案对比

### Android 方案对比

| 方案 | Root | 兼容性 | 难度 | 稳定性 | 推荐度 |
|------|------|--------|------|--------|--------|
| Xposed/LSPosed Hook | 需要 | Android 8+ | ⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| 虚拟摄像头 (VDM) | 不需要 | Android 14+ | ⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| Magisk + V4L2 | 需要 | 内核依赖 | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |

### iOS 方案对比

| 方案 | 越狱 | 兼容性 | 难度 | 稳定性 | 推荐度 |
|------|------|--------|------|--------|--------|
| Substrate/Theos | 需要 | iOS 12-16 | ⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| Frida 注入 | 不需要* | iOS 12+ | ⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐ |

> *Frida 非越狱注入需要重签名 App 或使用开发者证书

---

## 7. 开源项目参考

| 项目 | 类型 | 说明 |
|------|------|------|
| [HR-VITON](https://github.com/sangyun884/HR-VITON) | 虚拟试衣 | 高分辨率虚拟试衣，效果好 |
| [OOTDiffusion](https://github.com/levihsu/OOTDiffusion) | 虚拟试衣 | 基于 Diffusion 的试衣，质量高 |
| [IDM-VTON](https://github.com/yisol/IDM-VTON) | 虚拟试衣 | 最新 SOTA，效果出色 |
| [MediaPipe](https://github.com/google/mediapipe) | 人体分割/姿态 | Google 端到端方案，移动端友好 |
| [VirtualCamera](https://github.com/nickel-lang/virtual-camera-android) | 虚拟摄像头 | Android 虚拟摄像头参考实现 |
| [ncnn](https://github.com/Tencent/ncnn) | 推理框架 | 腾讯开源，移动端性能最佳 |
| [MNN](https://github.com/alibaba/MNN) | 推理框架 | 阿里开源，性能优秀 |

---

## 8. 注意事项

### 8.1 法律风险

> ⚠️ **重要提醒**：Hook 其他应用的摄像头数据可能涉及以下法律问题：
> - 侵犯用户隐私
> - 违反 App Store / Google Play 开发者协议
> - 可能触犯计算机犯罪相关法律
> 
> 请确保仅用于合法用途（如自己的设备测试、学术研究等）。

### 8.2 性能优化

- **分辨率**：移动端建议将处理分辨率降至 256x256 或 384x512
- **帧率**：实时 AI 换衣达到 30fps 有挑战，建议：
  - 使用轻量级模型（MobileNet 系列骨干网络）
  - 开启 GPU 加速（Vulkan/Metal/OpenCL）
  - 隔帧处理（每 2-3 帧处理一次，中间帧插值）
  - 使用 INT8 量化模型
- **内存**：注意移动端内存限制，模型大小建议控制在 50MB 以内

### 8.3 App 检测规避

- 部分 App 会检测 Xposed/Magisk/Frida 等框架并拒绝运行
- 应对措施：
  - 使用 MagiskHide / Shamiko 隐藏 Root
  - 使用 LSPosed 的应用作用域功能
  - Frida 使用 frida-server 改名 + 端口修改

### 8.4 推荐实施路线

```
阶段1：原型验证
├── 使用 MediaPipe 做人体分割 + 姿态估计
├── 简单的 Warping 换衣效果
└── 在独立 App 内验证效果

阶段2：Hook 集成
├── Android: 先用 Xposed Hook 验证可行性
├── iOS: 用 Frida 快速原型
└── 确认帧率和延迟是否可接受

阶段3：模型优化
├── 训练/转换轻量级换衣模型
├── INT8 量化 + GPU 加速
└── 优化到 20-30fps

阶段4：产品化
├── Android 14+ 迁移到虚拟摄像头方案（无需Root）
├── 完善 UI 和衣服选择功能
└── 稳定性和兼容性测试
```

---

*文档生成时间：2026年4月7日*
