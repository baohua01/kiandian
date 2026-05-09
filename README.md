# Win 桌面计算器

这是一个基于 **.NET 8 + Windows Forms** 的桌面计算器示例程序。

## 功能

- 四则运算：`+` `-` `×` `÷`
- 百分比：`%`
- 正负切换：`±`
- 退格：`⌫`
- 清空：`C`
- 小数输入与连续计算

## 运行环境

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## 如何运行

在项目根目录执行：

```bash
dotnet restore
dotnet run --project WinCalculator.csproj
```

## 打包发布（可选）

```bash
dotnet publish WinCalculator.csproj -c Release -r win-x64 --self-contained false
```

发布产物在：

`bin\Release\net8.0-windows\win-x64\publish\`
