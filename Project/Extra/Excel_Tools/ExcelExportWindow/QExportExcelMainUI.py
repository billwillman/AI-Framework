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
        if self.AllExcePaths != None:
            lubanPath = "../../../Tools/Luban/Luban.dll"
            self.process = QProcess(self)
            self.process.readyReadStandardOutput.connect(self.handle_stdout)
            self.process.readyReadStandardError.connect(self.handle_stderr)
            self.process.finished.connect(self.handle_finished)
            for path in self.AllExcePaths:
                path = path.replace('\\', '/')
                cmd = ("donet %s -t %s -c protobuf2 -d protobuf2-bin --conf ../../../Excel/luban.conf -x outputDataDir=../../../AIRebot/Assets/Resources/@Config/ -x outputCodeDir=./../../AIRebot/Assets/Resources/@PB_Config/") % (lubanPath, path)
                print(cmd)
                # 根据操作系统选择命令
                import os
                if os.name == 'nt':  # Windows
                    program = cmd
                    arguments = []
                else:  # Linux/Mac
                    program = cmd
                    arguments = []

                self.process.start(program, arguments)

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
        pass

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
