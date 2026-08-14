# MonthCalendar — журнал змін і контекст роботи

> Файл для швидкого відновлення контексту: новий агент читає його перед початком роботи,
> не досліджуючи проєкт заново. Після кожної сесії коротко дописуйте що зробили, що виправили і що далі.

---

## Актуальний стан (останнє оновлення: 13.08.2026)

**Проєкт:** плагін MonthCalendar для Loupedeck/Logi MX Creative Console.
Показує на кнопці поточний день/годинник або календар місяця; тап перемикає вигляд.

**Крос-платформність:** код зібрано й перевірено на Windows (0 помилок, 0 попереджень).
Проєкт спроєктований так, щоб один пакет працював і на Windows, і на macOS
(окремі підпапки `win/` і `mac/` у виводі збірки).

---

## Структура коду

- `src/Actions/CalendarCommandBase.cs` — абстрактна база: таймер оновлення (1 с у годиннику,
  1 хв у календарі), малювання `DrawDay` + `DrawCalendar`, вибір мови системи.
- `src/Actions/MonthCalendarCommand.cs` — екшен **Calendar**: мова системи, 24-годинний час,
  дата `dd.MM.yyyy`, укр. генітив місяців.
- `src/Actions/MonthCalendarUsaCommand.cs` — екшен **Calendar (USA)**: завжди `en-US`,
  12-годинний час `hh:mm tt`, дата `MM/dd/yyyy`.
- `src/MonthCalendarPlugin.cs` — клас плагіна, ініціалізація логу/ресурсів.
- `src/MonthCalendarApplication.cs` — заглушка клієнтського додатка (плагін не прив'язаний до додатка).
- `src/Helpers/PluginLog.cs`, `src/Helpers/PluginResources.cs` — стандартні хелпери.
- `src/MonthCalendarPlugin.csproj` — збірка в `bin/Release/win/` (Windows) і `bin/Release/mac/` (macOS);
  після збірки копіює service-бібліотеки та package-файли; перезапускає плагін у Logi Service
  (команда reload, тільки на Windows).
- `src/package/metadata/LoupedeckPackage.yaml` — `pluginFolderWin: win`, `pluginFolderMac: mac`.
- `src/package/metadata/DefaultIconTemplate.ict` — повноекранний малюнок без текстового оверлею.

---

## Хронологія змін

### 2026-08-14 — сесія: уточнення мовної поведінки

**Зміна (`src/Actions/CalendarCommandBase.cs`):**
- В `GetSystemCulture()` додано маппінг: певні мови системи, що не є підтримуваними,
  тепер відображаються українською (`uk-UA`).
- Плагін підтримує українську (за мовою системи), англійську (екшен "Calendar (USA)")
  та решту мов системи; непідтримувані мови відображаються українською.

**Перевірено:** `dotnet clean + build -c Release` — 0 помилок, 0 попереджень.
Пакет перепаковано, плагін перезавантажено — 2 dynamic actions loaded.

### 2026-08-14 — сесія: фікс завмирання годинника після сну/очікування

**Проблема:** після того, як ПК вийшов з режиму очікування, годинник на кнопці «завмер»
і переставав оновлюватися.

**Причина:** ланцюжок `OnRefresh → ActionImageChanged → ScheduleRefresh` був незахищеним.
Якщо будь-який виклик у колбеку таймера кидав виняток (найчастіше це стається саме при
відновленні системи), `ScheduleRefresh` не виконувався — таймер більше ніколи не
перепланувався, і годинник назавжди зупинявся. Також `GetCommandImage` не обробляв
збої малювання (Skia).

**Виправлення (`src/Actions/CalendarCommandBase.cs`):**
- `OnRefresh()`: обгорнуто в `try/catch/finally` — таймер перепланується у `finally`
  за будь-якого результату, помилка логується через `PluginLog.Error`.
- `GetCommandImage()`: обгорнуто в `try/catch`, при збої повертає заглушку `DrawError`
  (не показує порожню кнопку) і логує помилку.
- Додано `private BitmapImage DrawError(...)` — проста заглушка `:(`.

**Перевірено:** `dotnet clean + build -c Release` — 0 помилок, 0 попереджень.
Пакет перепаковано (`MonthCalendar.lplug4`), плагін перезавантажено — 2 dynamic actions loaded.

**Подальші кроки:**
- Протестувати на Windows: почекати засинання ПК, розбудити, перевірити що годинник оновлюється.
- Якщо повторюється — перевірити `MonthCalendar.log` на рядки "Calendar refresh failed" або
  "Calendar image drawing failed".

### 2026-08-13 — сесія: крос-платформна перевірка та виправлення

**Що було зроблено:**
- Прочитано весь код (базу, похідні команди, плагін, csproj, YAML, ict, хелпери).
- Зібрано проєкт на Windows `-c Release`: **Build succeeded, 0 Warning, 0 Error**.
- Перевірено лог служби: **2 dynamic actions loaded** (`MonthCalendarCommand`, `MonthCalendarUsaCommand`).
- Перевірено структуру виводу: `bin/Release/{win,mac,metadata,icontemplates}` відповідає YAML.

**Виправлення:**
- `src/Actions/CalendarCommandBase.cs` — `GetSystemCulture()`:
  - Раніше мова системи визначалась тільки через macOS-команду `/usr/bin/defaults read -g AppleLanguages`;
    на Windows це завжди падало в `catch` і відкочувалось до `CultureInfo.CurrentCulture`
    (у Logi Service він завжди en-US) → на українській Windows екшен "Calendar" показував би англійську.
  - Тепер: на `Win32NT` береться `CultureInfo.InstalledUICulture` (мова ОС), на macOS — як раніше
    через `defaults`. Поведінка на Mac не змінена.

**Перепакування:**
- `logiplugintool pack bin\Release MonthCalendar.lplug4` — успішно.

**Подальші кроки / на що звернути увагу:**
- На macOS: перезібрати та перевірити, що екшени визначають мову системи (шлях `/usr/bin/defaults`).
- Після збірки на Windows перезапустити Logi Options+ (Quit з трея), щоб з'явився екшен "Calendar (USA)".
- Кеш: `dotnet build` може використовувати закешовані бінарники — для чистої перевірки
  робити `dotnet clean` перед `build`.

---

## Як збирати та пакувати

```powershell
# Windows
dotnet build src\MonthCalendarPlugin.csproj -c Release
logiplugintool pack bin\Release MonthCalendar.lplug4
```

```bash
# macOS
dotnet build src/MonthCalendarPlugin.csproj -c Release
logiplugintool pack bin/Release MonthCalendar.lplug4
```

`logiplugintool` — глобальний .NET-інструмент (`dotnet tool install --global logiplugintool`).
