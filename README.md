# SetScreenBrightness 外接显示器亮度调节

This application can be used to adjust your screens' brightness and contrast (contrast adjust need right click tray icon to enable, and you can set it auto start with Windows by the way).

这款软件用于设置外接显示器的亮度和对比度（对比度设置需要右键托盘栏图标开启，也可以设置随开机自启动）。

![Snapshot](http://qiniu1.letow.top/snipaste%202018.12.13-03.32.jpg)

## attention

You can use it for your screen with the DDC/CI protocol enabled (usually it is enabled, you can use physical buttons on the screen to check). This application start supporting multiple monitors after 2.0.1 version.

只能用于启用了 DDC/DI 协议的显示器（通常是启用了的，你可以用显示器物理按键来打开）。从 2.0.1 版本后支持多屏幕。

You may need to right-click the tray icon and rescan monitors manually, if you remove or attach a monitor and the application failed to detect it (usually not happen).

如果移除或新增显示器后，程序没有自动刷新（小概率），你可以右键点击托盘栏图标，然后手动加载。

## Thanks to these projects

- [DDCMonitorManager](https://github.com/DeastinY/DDCMonitorManager)
- [Global-Low-Level-Key-Board-And-Mouse-Hook](https://github.com/rvknth043/Global-Low-Level-Key-Board-And-Mouse-Hook)

## Special thanks give to 特别感谢

- [Monitorian](https://github.com/emoacht/Monitorian)

After finding some big bugs in my project, I started reading this project's code and have learned too much from it, whose codes without doubts were largely copied into mine. （。＾▽＾）

It's terrible that windows API has so many confusing methods and overlapped classes, not to mention the unmodified same parameter name "hmonitor", which referred to both logic monitors and physical monitors. Perhaps this is operation system.