# MonthCalendar — clock and month calendar for Logitech MX Creative Console

A plugin (`.lplug4`) for Logitech MX Creative Console and compatible Loupedeck devices that puts a clock, today's date and a month calendar on a single button.

## Why this plugin exists

I built it for myself: I wanted the date and a month view always in front of me without switching windows. Nothing on the marketplace looked the way I wanted, so I wrote my own, with the help of AI.

## Features

- **Two views on one button** — tap to switch:
  - **Clock & date**: time, short weekday and date, full date.
  - **Month grid**: Monday-first calendar, today highlighted, weekends in accent color.
- **Calendar** action follows the system language (English, Ukrainian and others) with a 24-hour clock and `dd.MM.yyyy` dates.
- **Calendar (USA)** action always uses US formatting: 12-hour clock with AM/PM and `MM/dd/yyyy`.
- Redraws only when needed: every second for the 24-hour clock, once a minute for the 12-hour clock and the month grid.
- Survives sleep / resume — the refresh timer always reschedules itself.
- No internet access, no accounts, no data collection.

## Requirements

- Logitech MX Creative Console or a compatible Loupedeck device
- Logi Options+ with Logi Plugin Service
- Windows or macOS

## Installation

1. Download `MonthCalendar.lplug4` from the releases page.
2. Double-click the file — Logi Plugin Service installs the plugin automatically.
3. In Logi Options+, assign **Calendar** or **Calendar (USA)** to a button.

### Installation via command line

```powershell
logiplugintool install ./MonthCalendar.lplug4
```

## Building from source

Requires .NET 10 SDK and Logi Plugin Service (the build references its `PluginApi.dll` from the install folder).

```powershell
cd MonthCalendarPlugin
dotnet build -c Release
logiplugintool pack ./bin/Release ./MonthCalendar.lplug4
```

The Windows build goes to `bin/Release/win/`, the macOS build to `bin/Release/mac/`; build on each OS and pack both into one `.lplug4`. A Debug build also links the plugin into Logi Plugin Service and reloads it, so you can see changes on the device right away.

See [CHANGELOG.md](CHANGELOG.md) for the development history.

## Support the project

If you find this plugin useful and want to support development — a coffee donation is always welcome 🙂

[![Donate via PayPal](https://img.shields.io/badge/PayPal-Donate-00457C?style=for-the-badge&logo=paypal&logoColor=white)](https://www.paypal.com/donate/?business=donicteck%40gmail.com&currency_code=USD)

## License

[MIT](LICENSE) © 2026 GO
