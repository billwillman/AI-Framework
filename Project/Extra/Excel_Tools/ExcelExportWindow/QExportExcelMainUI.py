# This Python file uses the following encoding: utf-8
from PySide6 import QtCore
from PyQt6 import QtWidgets, uic
from PyQt6.QtGui import QStandardItemModel, QStandardItem
import os
import json
import tempfile
from pathlib import Path
from PyQt6.QtCore import QProcess

current_dir = os.path.dirname(os.path.abspath(__file__))
ui_file_path = os.path.join(current_dir, "mainwindow.ui")
Ui_MainWindow, baseClass = uic.loadUiType(ui_file_path)

class QExportExcelMainUI(baseClass, Ui_MainWindow):
    def __init__(self):
        super(QExportExcelMainUI, self).__init__()
        self.SelectExcelIndexes = []
        self.setupUi(self)
        # 获得所有EXCEL
        self.InitExcelList()
        # 设置按钮导出
        self.InitBtnExport()
        pass

    def handle_stdout(self):
        data = self.process.readAllStandardOutput()
        stdout = bytes(data).decode("utf8").strip()
        self.PrintLog(stdout)
        #self.output_log.append(stdout)

    def handle_stderr(self):
        data = self.process.readAllStandardError()
        stderr = bytes(data).decode("utf8").strip()
        if stderr:
            err = f"ERROR: {stderr}"
            self.PrintLog(err)
            #self.output_log.append(f"ERROR: {stderr}")

    def handle_finished(self):
        #self.output_log.append("命令执行完毕。")
        #self.btn_run.setEnabled(True)
        self.PrintLog("命令执行完毕。")

    def QBtnExport_OnClick(self):
        """导出选中的 Excel 数据到 protobuf2 格式

        为选中的表创建临时配置文件，然后只导出这些表
        生成 protobuf2 的 proto 文件（.proto）和二进制数据文件（.bytes）
        """
        self.ClearAllLog()
        if self.SelectExcelIndexes is None or len(self.SelectExcelIndexes) == 0:
            self.PrintLog("未选择任何 Excel 文件")
            return
        
        # 获取选中的 Excel 文件列表
        selected_files = []
        for idx in self.SelectExcelIndexes:
            excelPath = self.AllExcePaths[idx]
            tableName = Path(excelPath).stem
            if tableName.startswith("#"):
                tableName = tableName[1:]
            selected_files.append(tableName)
        
        self.PrintLog(f"选中的表: {', '.join(selected_files)}")
        
        # 获取当前脚本所在目录
        script_dir = os.path.dirname(os.path.abspath(__file__))
        
        # Luban 工具路径（使用绝对路径）
        lubanPath = os.path.normpath(os.path.join(script_dir, "../../../Tools/Luban/Luban.dll"))
        lubanPath = os.path.abspath(lubanPath)
        
        # 原始配置文件路径
        originalConfPath = os.path.normpath(os.path.join(script_dir, "../../../Excel/luban.conf"))
        originalConfPath = os.path.abspath(originalConfPath)
        
        # 输出目录
        outputDataDir = os.path.normpath(os.path.join(script_dir, "../../../AIRebot/Assets/Resources/@Config/"))
        outputDataDir = os.path.abspath(outputDataDir)
        outputCodeDir = os.path.normpath(os.path.join(script_dir, "../../../AIRebot/Assets/Resources/@PB_Config/"))
        outputCodeDir = os.path.abspath(outputCodeDir)
        
        # 确保输出目录存在
        os.makedirs(outputDataDir, exist_ok=True)
        os.makedirs(outputCodeDir, exist_ok=True)
        
        # 创建临时配置文件
        tempConfPath = self.CreateTempConfig(originalConfPath, selected_files)
        if tempConfPath is None:
            self.PrintLog("创建临时配置文件失败")
            return
        
        self.PrintLog(f"使用临时配置文件: {tempConfPath}")
        
        # 构建 Luban 命令参数
        # -t: 指定目标（client/server/all）
        # -c: 指定代码类型（protobuf2）
        # -d: 指定数据类型（protobuf2-bin 表示 protobuf2 二进制格式）
        # --conf: 指定配置文件
        # -x outputDataDir: 输出数据目录
        # -x outputCodeDir: 输出代码目录
        
        arguments = [
            lubanPath,
            "-t", "all",  # 使用 all 目标，生成所有分组的数据
            "-c", "protobuf2",  # 生成 protobuf2 的 proto 文件
            "-d", "protobuf2-bin",  # 生成 protobuf2 二进制数据
            "--conf", tempConfPath,
            "-x", f"outputDataDir={outputDataDir}",
            "-x", f"outputCodeDir={outputCodeDir}",
        ]
        
        self.PrintLog("=" * 50)
        self.PrintLog("开始执行 Luban 导出命令...")
        self.PrintLog(f"Luban 路径: {lubanPath}")
        self.PrintLog(f"配置文件: {tempConfPath}")
        self.PrintLog(f"数据输出目录: {outputDataDir}")
        self.PrintLog(f"代码输出目录: {outputCodeDir}")
        self.PrintLog("=" * 50)
        
        # 初始化进程
        self.process = QProcess(self)
        self.process.readyReadStandardOutput.connect(self.handle_stdout)
        self.process.readyReadStandardError.connect(self.handle_stderr)
        self.process.finished.connect(self.handle_finished)
        
        # 使用 dotnet 运行 Luban.dll
        program = "dotnet"
        
        self.PrintLog(f"执行命令: {program} {' '.join(arguments)}")
        self.PrintLog("")
        
        # 启动进程
        self.process.start(program, arguments)
        
        # 等待进程完成（使用较长的超时时间，因为可能需要处理多个文件）
        if not self.process.waitForFinished(300000):  # 5分钟超时
            self.PrintLog("Luban 导出超时")
        else:
            exit_code = self.process.exitCode()
            if exit_code == 0:
                self.PrintLog("")
                self.PrintLog("=" * 50)
                self.PrintLog("Luban 导出成功完成！")
                self.PrintLog(f"Proto 文件位置: {outputCodeDir}")
                self.PrintLog(f"二进制数据位置: {outputDataDir}")
                self.PrintLog("=" * 50)
            else:
                self.PrintLog(f"Luban 导出失败，退出码: {exit_code}")
        
        # 清理临时文件
        try:
            if os.path.exists(tempConfPath):
                os.remove(tempConfPath)
                self.PrintLog(f"已清理临时配置文件: {tempConfPath}")
        except Exception as e:
            self.PrintLog(f"清理临时文件失败: {str(e)}")
        
        return

    def CreateTempConfig(self, originalConfPath, selectedTables):
        """创建临时配置文件，只包含选中的表
        
        Args:
            originalConfPath: 原始配置文件路径
            selectedTables: 选中的表名列表
            
        Returns:
            临时配置文件路径，失败返回 None
        """
        try:
            # 读取原始配置文件
            if not os.path.exists(originalConfPath):
                self.PrintLog(f"原始配置文件不存在: {originalConfPath}")
                return None
            
            with open(originalConfPath, 'r', encoding='utf-8') as f:
                config = json.load(f)
            
            # 修改 schemaFiles，只包含选中的表
            originalSchemaFiles = config.get('schemaFiles', [])
            newSchemaFiles = []
            
            for schemaFile in originalSchemaFiles:
                schemaName = schemaFile.get('fileName', '')
                # 检查这个 schema 是否在选中的表列表中
                for tableName in selectedTables:
                    # 如果表名完全匹配，或者包含表名（比如 schema 名为 "Table"，表名为 "Table"）
                    if tableName == schemaName or tableName.startswith(schemaName):
                        newSchemaFiles.append(schemaFile)
                        break
            
            # 更新配置
            config['schemaFiles'] = newSchemaFiles
            
            # 同时也更新 targets，确保只使用选中的表
            # 如果没有选中的 schemaFiles，使用原配置
            if len(newSchemaFiles) == 0:
                self.PrintLog(f"警告: 未找到选中的表对应的 schema 文件")
                self.PrintLog(f"选中的表: {selectedTables}")
                self.PrintLog(f"原始 schemaFiles: {[s.get('fileName', '') for s in originalSchemaFiles]}")
            
            # 创建临时文件
            tempDir = tempfile.gettempdir()
            tempFileName = f"luban_temp_conf_{os.getpid()}_{len(selectedTables)}tables.conf"
            tempConfPath = os.path.join(tempDir, tempFileName)
            
            # 写入临时配置文件
            with open(tempConfPath, 'w', encoding='utf-8') as f:
                json.dump(config, f, indent='\t', ensure_ascii=False)
            
            self.PrintLog(f"已创建临时配置文件")
            self.PrintLog(f"  原始 schema 数量: {len(originalSchemaFiles)}")
            self.PrintLog(f"  过滤后 schema 数量: {len(newSchemaFiles)}")
            
            return tempConfPath
            
        except Exception as e:
            self.PrintLog(f"创建临时配置文件失败: {str(e)}")
            import traceback
            self.PrintLog(traceback.format_exc())
            return None

    def ClearAllLog(self):
        self.textBrowser.setText("")
        return

    def PrintLog(self, str):
        print(str)
        if self.textBrowser != None:
            self.textBrowser.append(str)
        return

    def InitBtnExport(self):
        self.QBtnExport.clicked.connect(self.QBtnExport_OnClick)
        return

    def InitExcelList(self):
        dir = os.path.dirname(os.path.abspath(__file__)) + "/../../../Excel"
        if os.path.exists(dir) and os.path.isdir(dir):
            dir = os.path.abspath(dir)
            self.AllExcePaths = self.GetAllExcelFilePath(dir)
            if self.AllExcePaths != None and self.QExcelList != None:
                model = QStandardItemModel()
                for path in self.AllExcePaths:
                    name = os.path.basename(path)
                    if name != None:
                        if name.startswith("#"):
                            name = name[1:]
                        item = QStandardItem(name)
                        item.setCheckable(True)
                        model.appendRow(item)
                self.QExcelList.setModel(model)
                self.MExcelList = model
                self.MExcelList.itemChanged.connect(self.OnExcelItemChanged)
        pass

    def OnExcelItemChanged(self, item):
        if item.checkState().value == 2:
            self.SelectExcelIndexes.append(item.index().row())
        elif item.checkState().value == 0:
            self.SelectExcelIndexes.remove(item.index().row())
        return

    ## 获得所有EXCEL文件路径
    def GetAllExcelFilePath(self, root_directory):
        """
            递归搜索 root_directory 及其子目录下所有的 Excel 文件路径。
            支持 .xlsx, .xls, .xlsm, .xltx, .xltm 等常见格式。
            """
        root_path = Path(root_directory)

        # 定义 Excel 文件扩展名
        excel_extensions = ('.xlsx', '.xls', '.xlsm', '.xltx', '.xltm')

        # 使用 rglob 进行递归搜索
        excel_files = []
        for ext in excel_extensions:
            excel_files.extend(root_path.rglob(f'*{ext}'))

        # 转换为字符串路径列表并返回
        return [str(file.resolve()) for file in excel_files]
