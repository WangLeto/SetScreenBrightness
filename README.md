# SetScreenBrightness 外接显示器亮度调节

This application can set your external screen's brightness and contrast (contrast adjust need right click tray icon to enable), and you can set it auto start with your windows.

这款软件用于设置外接显示器的亮度和对比度（对比度设置需要右键托盘栏图标开启），也可以设置随开机自启动。

![Snapshot](http://qiniu1.letow.top/snipaste%202018.11.22-20.00.jpg)

## attention

You can only use it for your external screen which turn the DDC/CI protocol on (usually enabled, you can use physical buttons on screen to check), and this application only work for one screen (because I don't have two).

只能用于启用了 DDC/DI 协议的显示器（通常是启用了的，你可以用显示器物理按键来打开）。这个应用只能调节一个屏幕的亮度，因为我没有双屏。

## Thanks to these repo

- [DDCMonitorManager](https://github.com/DeastinY/DDCMonitorManager)
- [Global-Low-Level-Key-Board-And-Mouse-Hook](https://github.com/rvknth043/Global-Low-Level-Key-Board-And-Mouse-Hook)

## Special thanks give to 特别感谢

- [Monitorian](https://github.com/emoacht/Monitorian)

After some big bugs being found, I have learned too much from this repository, whose codes without doubts were largely copied into mine. （。＾▽＾）

It's terrible that windows API has so many confusing methods and overlapped classes, not to mention the unmodified same parameter name "hmonitor", which referred to both logic monitors and physical monitors. Perhaps this is operation system.