# This Python file uses the following encoding: utf-8
from PySide6 import QtCore
from PyQt6 import QtWidgets, uic
import os

current_dir = os.path.dirname(os.path.abspath(__file__))
ui_file_path = os.path.join(current_dir, "mainwindow.ui")
Ui_MainWindow, baseClass = uic.loadUiType(ui_file_path)

class QExportExcelMainUI(baseClass, Ui_MainWindow):
    def __init__(self):
        super(QExportExcelMainUI, self).__init__()
        self.setupUi(self)
        pass
