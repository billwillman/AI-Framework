# This Python file uses the following encoding: utf-8
from PySide6 import QtCore
from PyQt6 import QtWidgets, uic
from PyQt6.QtGui import QStandardItemModel, QStandardItem
import os
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
        print(stdout)
        #self.output_log.append(stdout)

    def handle_stderr(self):
        data = self.process.readAllStandardError()
        stderr = bytes(data).decode("utf8").strip()
        if stderr:
            err = f"ERROR: {stderr}"
            print(err)
            #self.output_log.append(f"ERROR: {stderr}")

    def handle_finished(self):
        #self.output_log.append("命令执行完毕。")
        #self.btn_run.setEnabled(True)
        print("命令执行完毕。")

    def QBtnExport_OnClick(self):
        """导出 Excel 数据到 protobuf2 格式
        
        Luban 工具会根据 luban.conf 配置批量处理 Excel 目录中的所有文件，
        生成 protobuf2 的 proto 文件（.proto）和二进制数据文件（.bytes）
        """
        if self.SelectExcelIndexes is None or len(self.SelectExcelIndexes) == 0:
            print("未选择任何 Excel 文件")
            return
        
        # 获取选中的 Excel 文件列表
        selected_files = []
        for idx in self.SelectExcelIndexes:
            excelPath = self.AllExcePaths[idx]
            tableName = Path(excelPath).stem
            if tableName.startswith("#"):
                tableName = tableName[1:]
            selected_files.append(tableName)
        
        print(f"选中的表: {', '.join(selected_files)}")
        
        # 获取当前脚本所在目录
        script_dir = os.path.dirname(os.path.abspath(__file__))
        
        # Luban 工具路径（使用绝对路径）
        lubanPath = os.path.normpath(os.path.join(script_dir, "../../../Tools/Luban/Luban.dll"))
        lubanPath = os.path.abspath(lubanPath)
        
        # 配置文件路径
        confPath = os.path.normpath(os.path.join(script_dir, "../../../Excel/luban.conf"))
        confPath = os.path.abspath(confPath)
        
        # 输出目录
        outputDataDir = os.path.normpath(os.path.join(script_dir, "../../../AIRebot/Assets/Resources/@Config/"))
        outputDataDir = os.path.abspath(outputDataDir)
        outputCodeDir = os.path.normpath(os.path.join(script_dir, "../../../AIRebot/Assets/Resources/@PB_Config/"))
        outputCodeDir = os.path.abspath(outputCodeDir)
        
        # 确保输出目录存在
        os.makedirs(outputDataDir, exist_ok=True)
        os.makedirs(outputCodeDir, exist_ok=True)
        
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
            "--conf", confPath,
            "-x", f"outputDataDir={outputDataDir}",
            "-x", f"outputCodeDir={outputCodeDir}",
        ]
        
        print("=" * 50)
        print("开始执行 Luban 导出命令...")
        print(f"Luban 路径: {lubanPath}")
        print(f"配置文件: {confPath}")
        print(f"数据输出目录: {outputDataDir}")
        print(f"代码输出目录: {outputCodeDir}")
        print("=" * 50)
        
        # 初始化进程
        self.process = QProcess(self)
        self.process.readyReadStandardOutput.connect(self.handle_stdout)
        self.process.readyReadStandardError.connect(self.handle_stderr)
        self.process.finished.connect(self.handle_finished)
        
        # 使用 dotnet 运行 Luban.dll
        program = "dotnet"
        
        print(f"执行命令: {program} {' '.join(arguments)}")
        print("")
        
        # 启动进程
        self.process.start(program, arguments)
        
        # 等待进程完成（使用较长的超时时间，因为可能需要处理多个文件）
        if not self.process.waitForFinished(300000):  # 5分钟超时
            print("Luban 导出超时")
        else:
            exit_code = self.process.exitCode()
            if exit_code == 0:
                print("")
                print("=" * 50)
                print("Luban 导出成功完成！")
                print(f"Proto 文件位置: {outputCodeDir}")
                print(f"二进制数据位置: {outputDataDir}")
                print("=" * 50)
            else:
                print(f"Luban 导出失败，退出码: {exit_code}")
        
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
