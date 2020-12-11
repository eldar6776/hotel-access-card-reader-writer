/****************************************************************************
** Meta object code from reading C++ file 'MainWindow.h'
**
** Created: Wed Jun 15 13:36:23 2011
**      by: The Qt Meta Object Compiler version 62 (Qt 4.7.0)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../../../HIDBoot/Bootloader/MainWindow.h"
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'MainWindow.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 62
#error "This file was generated using the moc from 4.7.0. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
static const uint qt_meta_data_MainWindow[] = {

 // content:
       5,       // revision
       0,       // classname
       0,    0, // classinfo
      19,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       4,       // signalCount

 // signals: signature, parameters, type, tag, flags
      22,   12,   11,   11, 0x05,
      80,   76,   11,   11, 0x05,
     109,   76,   11,   11, 0x05,
     140,  131,   11,   11, 0x05,

 // slots: signature, parameters, type, tag, flags
     160,   11,   11,   11, 0x0a,
     173,   11,   11,   11, 0x0a,
     190,   12,   11,   11, 0x0a,
     243,   76,   11,   11, 0x0a,
     270,   76,   11,   11, 0x0a,
     301,  131,   11,   11, 0x0a,
     324,   11,   11,   11, 0x08,
     357,   11,   11,   11, 0x08,
     391,   11,   11,   11, 0x08,
     422,   11,   11,   11, 0x08,
     458,   11,   11,   11, 0x08,
     486,   11,   11,   11, 0x08,
     520,   11,   11,   11, 0x08,
     546,   11,   11,   11, 0x08,
     580,   11,   11,   11, 0x08,

       0        // eod
};

static const char qt_meta_stringdata_MainWindow[] = {
    "MainWindow\0\0msg,,time\0"
    "IoWithDeviceCompleted(QString,Comm::ErrorCode,double)\0"
    "msg\0IoWithDeviceStarted(QString)\0"
    "AppendString(QString)\0newValue\0"
    "SetProgressBar(int)\0Connection()\0"
    "openRecentFile()\0"
    "IoWithDeviceComplete(QString,Comm::ErrorCode,double)\0"
    "IoWithDeviceStart(QString)\0"
    "AppendStringToTextbox(QString)\0"
    "UpdateProgressBar(int)\0"
    "on_actionBlank_Check_triggered()\0"
    "on_actionReset_Device_triggered()\0"
    "on_action_Settings_triggered()\0"
    "on_action_Verify_Device_triggered()\0"
    "on_action_About_triggered()\0"
    "on_actionWrite_Device_triggered()\0"
    "on_actionOpen_triggered()\0"
    "on_actionErase_Device_triggered()\0"
    "on_actionExit_triggered()\0"
};

const QMetaObject MainWindow::staticMetaObject = {
    { &QMainWindow::staticMetaObject, qt_meta_stringdata_MainWindow,
      qt_meta_data_MainWindow, 0 }
};

#ifdef Q_NO_DATA_RELOCATION
const QMetaObject &MainWindow::getStaticMetaObject() { return staticMetaObject; }
#endif //Q_NO_DATA_RELOCATION

const QMetaObject *MainWindow::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->metaObject : &staticMetaObject;
}

void *MainWindow::qt_metacast(const char *_clname)
{
    if (!_clname) return 0;
    if (!strcmp(_clname, qt_meta_stringdata_MainWindow))
        return static_cast<void*>(const_cast< MainWindow*>(this));
    return QMainWindow::qt_metacast(_clname);
}

int MainWindow::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QMainWindow::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        switch (_id) {
        case 0: IoWithDeviceCompleted((*reinterpret_cast< QString(*)>(_a[1])),(*reinterpret_cast< Comm::ErrorCode(*)>(_a[2])),(*reinterpret_cast< double(*)>(_a[3]))); break;
        case 1: IoWithDeviceStarted((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 2: AppendString((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 3: SetProgressBar((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 4: Connection(); break;
        case 5: openRecentFile(); break;
        case 6: IoWithDeviceComplete((*reinterpret_cast< QString(*)>(_a[1])),(*reinterpret_cast< Comm::ErrorCode(*)>(_a[2])),(*reinterpret_cast< double(*)>(_a[3]))); break;
        case 7: IoWithDeviceStart((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 8: AppendStringToTextbox((*reinterpret_cast< QString(*)>(_a[1]))); break;
        case 9: UpdateProgressBar((*reinterpret_cast< int(*)>(_a[1]))); break;
        case 10: on_actionBlank_Check_triggered(); break;
        case 11: on_actionReset_Device_triggered(); break;
        case 12: on_action_Settings_triggered(); break;
        case 13: on_action_Verify_Device_triggered(); break;
        case 14: on_action_About_triggered(); break;
        case 15: on_actionWrite_Device_triggered(); break;
        case 16: on_actionOpen_triggered(); break;
        case 17: on_actionErase_Device_triggered(); break;
        case 18: on_actionExit_triggered(); break;
        default: ;
        }
        _id -= 19;
    }
    return _id;
}

// SIGNAL 0
void MainWindow::IoWithDeviceCompleted(QString _t1, Comm::ErrorCode _t2, double _t3)
{
    void *_a[] = { 0, const_cast<void*>(reinterpret_cast<const void*>(&_t1)), const_cast<void*>(reinterpret_cast<const void*>(&_t2)), const_cast<void*>(reinterpret_cast<const void*>(&_t3)) };
    QMetaObject::activate(this, &staticMetaObject, 0, _a);
}

// SIGNAL 1
void MainWindow::IoWithDeviceStarted(QString _t1)
{
    void *_a[] = { 0, const_cast<void*>(reinterpret_cast<const void*>(&_t1)) };
    QMetaObject::activate(this, &staticMetaObject, 1, _a);
}

// SIGNAL 2
void MainWindow::AppendString(QString _t1)
{
    void *_a[] = { 0, const_cast<void*>(reinterpret_cast<const void*>(&_t1)) };
    QMetaObject::activate(this, &staticMetaObject, 2, _a);
}

// SIGNAL 3
void MainWindow::SetProgressBar(int _t1)
{
    void *_a[] = { 0, const_cast<void*>(reinterpret_cast<const void*>(&_t1)) };
    QMetaObject::activate(this, &staticMetaObject, 3, _a);
}
QT_END_MOC_NAMESPACE
