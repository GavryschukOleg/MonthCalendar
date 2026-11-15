# MonthCalendar — контекст роботи для агентів і журнал розробки

> Файл для швидкого відновлення контексту: новий агент читає його перед початком роботи,
> не досліджуючи проєкт заново. Після кожної сесії коротко дописуйте що зробили, що виправили і що далі.
> Для користувачів — `README.md`. Нотатки до релізів — у GitHub Releases (окремого CHANGELOG немає, як і в HardwareMonitor).

---

## Актуальний стан (останнє оновлення: 15.11.2026)

**Проєкт:** плагін MonthCalendar для Loupedeck/Logi MX Creative Console.
Показує на кнопці годинник із датою або календар місяця; тап перемикає вигляд. Назва плагіна для користувача — **ClockCalendar**
(`displayName` в yaml); технічні ідентифікатори (`name: MonthCalendar`, `MonthCalendarPlugin.dll`, простір імен, репозиторій) лишилися
старими, щоб не ламати кнопки користувачів і не плутати збірки.
Версія пакета: **1.1** (`LoupedeckPackage.yaml`). Випущено на GitHub (реліз `v1.1`, репозиторій публічний, гілка `main`);
далі — Logi Marketplace.
Дванадцять дизайнів у стилі плагіна HardwareMonitor (Classic, Ring, Minimal, Date, Analog, Orbit, Week, Stack, Segment, Flip, Binary, Moon),
кожен у двох версіях — разом 24 actions, розкладених у **дві групи** в списку дій Logi: `Clock Calendar` (системна мова, 24 год)
і `Clock Calendar (AM/PM)` (англійська, 12 год, дати США). Назви actions — просто назва дизайну (`Ring`, `Ring (AM/PM)` тощо).
Класи `…UsaCommand` — це AM/PM-версії (назву класу не міняти: це ідентифікатор дії в профілях).

**Платформи:** пакет двоплатформовий: у yaml є `pluginFolderWin: win` і `pluginFolderMac: mac`. Кожну частину збирають на своїй ОС
(Windows → `bin/Release/win`, macOS → `bin/Release/mac`), в один пакет їх зводять разом. `logiplugintool verify` не пропускає пакет,
поки немає однієї з папок, тому `pack`/`verify` запускають тільки коли є обидві. Щоденна розробка на Windows цим не страждає.
Код не залежить від ОС (є гілка для macOS у `SystemCulture`). Пакет 1.1 перевірено на обох ОС, на пристрої — працює.

---

## Структура коду

- `src/Actions/CalendarCommandBase.cs` — абстрактна база: таймер оновлення (1 с, якщо стиль показує секунди,
  інакше 1 хв), підготовка даних (`DayInfo` / `MonthInfo`) і виклик стилю. Саме малювання тут не робиться.
- `src/Actions/MonthCalendarCommand.cs` — екшен **Classic** (група `Clock Calendar`; у 1.0 називався «Calendar»): мова системи, 24-годинний час,
  дата `dd.MM.yyyy`, укр. скорочені назви місяців. Стиль **Classic**.
- `src/Actions/MonthCalendarUsaCommand.cs` — екшен **Classic (AM/PM)** (група `Clock Calendar (AM/PM)`; у 1.0 «Calendar (USA)»): завжди `en-US`,
  12-годинний час `h:mm tt`, дата `MM/dd/yyyy`. Стиль **Classic**.
- `src/Actions/CalendarStyleCommands.cs` — решта 22 actions: `<Дизайн>`
  і `<Дизайн> (AM/PM)`. Кожен — кілька рядків (файл згенеровано скриптом, але редагується вручну): успадковує `MonthCalendarCommand` або
  `MonthCalendarUsaCommand` і перевизначає `Style`. Імена класів `MonthCalendarCommand` / `MonthCalendarUsaCommand`
  не змінювати: користувачі 1.0 мають їх на кнопках.
- `src/Rendering/Canvas.cs` — полотно в стилі HardwareMonitor: сітка 180×180 одиниць, що масштабується під кнопку
  (`X/Y/L/Font`), точний вимір тексту (`Measure`, `FitFont`, таблиця `Advance`) і `Label` з вирівнюванням за
  верхом великих літер; `Palette` — кольори (ті самі, що у HardwareMonitor + зелений акцент `#33C44C`).
- `src/Rendering/CalendarStyle.cs` — `DayInfo`, `MonthInfo`, абстрактний `CalendarStyle` і реєстр `CalendarStyles`.
- `src/Rendering/MonthGrid.cs` — спільна сітка місяця й `DrawStandard` (шапка ліворуч, диск «сьогодні»); стилі відрізняються шапкою, літерами днів тижня
  і маркером «сьогодні»: диск / кільце / квадрат / лише текст.
- `src/Rendering/ClassicStyle.cs`, `RingStyle.cs`, `MinimalStyle.cs`, `DateStyle.cs`, `AnalogStyle.cs`, `OrbitStyle.cs`, `WeekStyle.cs`,
  `StackStyle.cs`, `SegmentStyle.cs`, `FlipStyle.cs`, `BinaryStyle.cs`, `MoonStyle.cs` — по стилю: `DrawDay` + `DrawMonth`
  (місяць у багатьох стилів — спільний `MonthGrid.DrawStandard` або делегування іншому стилю). `TicksEverySecond` = чи показує
  стиль секунди (інакше кнопка оновлюється раз на хвилину).
  Додати новий стиль = новий клас + 2 actions у `CalendarStyleCommands.cs`.
- `src/Helpers/SystemCulture.cs` — визначення мови ОС (Windows: `InstalledUICulture`,
  macOS: `defaults read -g AppleLanguages`; російська → українська).
- `src/Helpers/PluginLog.cs` — обгортка над логом плагіна.
- `src/MonthCalendarPlugin.cs` — клас плагіна (без прив'язки до додатка).
- `src/MonthCalendarApplication.cs` — порожній `ClientApplication`. НЕ ВИДАЛЯТИ: без цього класу
  Logi Service не завантажує плагін (в логу лише «Cannot load plugin…» і плагін потрапляє в disabled-список).
- `src/MonthCalendarPlugin.csproj` — збірка в `bin/Release/win/` (Windows) і `bin/Release/mac/` (macOS);
  після збірки копіює package-файли; перезапускає плагін у Logi Service (команда reload).
  Вивід збірки містить лише `MonthCalendarPlugin.dll` + `deps.json`: бібліотеки Logi Service
  (`PluginApi.dll`, Skia тощо) НЕ копіюються — сервіс завантажує свої.
- `src/package/metadata/LoupedeckPackage.yaml` — метадані пакета (`pluginFolderWin: win`, `pluginFolderMac: mac`).
- `src/package/metadata/Icon256x256.png` — іконка плагіна; вихідник — `docs/images/icon.svg`
  (рендер у PNG будь-яким SVG-інструментом, напр. `rsvg-convert -w 256`).
- `docs/images/` — `hero.png` і по картинці на дизайн (`bar/ring/minimal/date.png`) для README, `icon.svg` (вихідник іконки). Картинки — реальний вивід плагіна (див. «Як перевіряли дизайн без пристрою»).
- `.github/FUNDING.yml` — кнопка Sponsor (PayPal), як у HardwareMonitor.

**Фактичні розміри кнопок:** `PluginImageSize.Width60` = 50×50 px, `Width90` = 80×80, `Width116` = 116×116.
На 50 px місяць тісний (7 колонок по 2 цифри), літери днів тижня там не показуються.

---

## Хронологія змін

### 2026-11-15 — сесія: збірка macOS, двоплатформовий пакет, реліз v1.1 на GitHub

**Збірка на macOS:** `dotnet build -c Release` (SDK 10.0.401) — 0 попереджень, 0 помилок; `bin/Release/mac/` поруч із
готовою `win/` з Windows. Плагін завантажився в Logi Plugin Service на Маці з репозиторію: `24 dynamic actions loaded`,
`version '1.1'`. Змін коду під macOS не знадобилося: `SystemCulture` правильно читає `defaults read -g AppleLanguages`
(`"uk-UA"`). Автор перевірив пакет на пристрої і на Windows, і на macOS — працює.

**Пакет:** `MonthCalendar_1_1.lplug4`, згодом перейменований на `ClockCalendar_1_1.lplug4` (43 706 байт: `win/`, `mac/`, `metadata/`), `logiplugintool verify` — OK.

**Реліз на GitHub (як у HardwareMonitor):** тег `v1.1` на коміті «Release 1.1», заголовок `ClockCalendar 1.1`, пакет
прикріплено. Репозиторій зроблено публічним, додано опис, гілка за замовчуванням — `main`; гілку `master` (лише initial
commit) видалено. Гілку `marketplace-prep` (стара підготовка 1.1 від 02.11, останній коміт `b091cae`)
теж видалено, після того як її виправлення перенесли в `main` (коміт `2760632`). На GitHub лишилась тільки `main`.
README: macOS тепер підтримується (бейдж, вимоги, шлях до логу на Маці), розмір пакета «under 50 KB».

**Корисне, що з'ясовано (macOS):**
- `logiplugintool` 6.1.4 зібраний під .NET 8; якщо встановлено лише .NET 10, він не запускається. Рішення:
  `DOTNET_ROLL_FORWARD=Major logiplugintool …` (або поставити .NET 8 runtime). Інструмент лежить у `~/.dotnet/tools`,
  який треба додати в `PATH`.
- На macOS `.csproj` пише в `MonthCalendarPlugin.link` шлях із пробілом у кінці (через `\` перед `>` у команді `echo`);
  сервіс це приймає, плагін вантажиться. Шлях до репозиторію на Маці теж містить пробіл (`Vibecoding Apps /`) — не заважає.
- У `~/Library/Application Support/Logi/LogiPluginService/Plugins/MonthCalendar/` лежала стара встановлена 1.0 (від 13.08,
  35 МБ); сервіс усе одно брав версію через лінк. Її перенесено в Кошик; після reload плагін 1.1 вантажиться з 24 actions.
- Лог плагіна на Маці: `~/Library/Application Support/Logi/LogiPluginService/Logs/plugin_logs/MonthCalendar.log`.

**Перейменування пакета:** файл релізу тепер `ClockCalendar_1_1.lplug4` (вміст не змінено, SHA-256 той самий); у релізі
`v1.1` старий файл `MonthCalendar_1_1.lplug4` замінено новим. Технічна назва плагіна (`name: MonthCalendar`) не змінилась.

**Увага:** коміт `2760632` (виправлення таймера після unload, місяць завжди відкривається на поточному) зроблено вже після
тегу `v1.1` — у пакеті 1.1 його немає. Він увійде в наступну версію (зібрати win + mac).

**Подальші кроки:**
- Випустити наступну версію з комітом `2760632`.
- Подати плагін у Logi Marketplace.
- Компактніший вигляд місяця для кнопок 50 px, якщо такі пристрої є.
- Ідея: обертання диска для перегортання місяців (потрібен `PluginDynamicAdjustment`).

### 2026-11-12 — сесія: дизайн у стилі HardwareMonitor, 4 варіанти як окремі actions, підготовка до релізу 1.1

**Дизайн.** Спочатку був зроблений кольоровий градієнтний дизайн, але його замінили: за побажанням — лаконічний і в одному
стилі з плагіном HardwareMonitor. Стиль HardwareMonitor витягнуто декомпіляцією його `HardwareMonitorPlugin.dll`
(`ilspycmd`, власний плагін автора): чорний фон `BitmapColor.Black`, плоскі елементи, палітра
`Gray 158,158,158` / `Light 201,206,214` / `Track 35,39,46` / `Green 51,196,76`, сітка 180 одиниць,
шрифт Logi за замовчуванням (назви шрифтів у плагіні не задаються), вимір тексту таблицею `Advance`.
Спершу зроблено 4 варіанти (Bar, Ring, Minimal, Date), потім ще 11 кандидатів; автор обрав 12 (див. нижче). Усі збережено як окремі actions.

**Вибір дизайнів (кінцевий).** Усього було 15 кандидатів. Залишено 12: Classic, Ring, Minimal, Date, Analog, Orbit, Week, Stack,
Segment, Flip, Binary, Moon. Видалено Bar, Progress (день/місяць/рік смужками) і Arc (дуга доби) — код і зайві поля `DayInfo`
прибрано. Оскільки Bar був дефолтом `Calendar` / `Calendar (USA)`, цим двом actions (їх мають користувачі 1.0) тепер відповідає
**Classic** — найближчий до старого вигляду (час із секундами + дата). Кожен дизайн має USA-версію (12-годинний час, `en-US`).
Фаза місяця (`Moon`) рахується від новомісяччя 2000-01-06 18:14 UTC за середнім синодичним місяцем (29.530588853 доби), для
північної півкулі (освітлена права сторона при зростанні).

**Корисне, що з'ясовано:**
- `BitmapImage.FromSvg` поза Logi Service повертає порожнє зображення (-1x-1) — не покладатися на SVG для кадрів кнопки.
- Рендер тексту переносить довгі рядки, а не обрізає їх → ширину завжди міряти й підганяти (`Canvas.FitFont`).
- Мінімальний шрифт у стилі HardwareMonitor — 10 px, через що на 80 px повна назва місяця + рік іноді не вміщуються
  і береться скорочення (`CalendarStyle.FitMonthName`).
- Початкові пробіли в тексті рендер ігнорує — розділювачі малювати окремим `Label` зі зсувом.
- Фактичні розміри кнопок: `Width60` = 50 px, `Width90` = 80 px, `Width116` = 116 px.
- Після перенесення репозиторію в іншу теку (чи сплощення структури) Logi Service ще тримає плагін зі старого
  шляху: reload дає «plugin is already loaded». Потрібно вийти з Logi Options+ (Quit з трея) і запустити знову.
- Завжди запускати `logiplugintool verify` перед релізом — він виявив, що попередні пакети оголошували macOS
  без папки `mac/`.

**Прибирання:**
- Видалено `PluginResources.cs` (не використовувався), невикористані методи `PluginLog`, `.DS_Store`,
  вкладений `.gitignore` (дублював кореневий). `MonthCalendarApplication.cs` спершу теж видалили, але це
  зламало завантаження плагіна в Logi Service — файл повернуто (див. структуру коду).
- Прибрано target `CopyServiceLibraries` з `.csproj` і додано `<Private>false</Private>` для `PluginApi`:
  пакет був ~9.5 МБ, бо містив копію бібліотек Logi Service; тепер ~26 КБ. Перевірено, що плагін завантажується без
  них. Для Release вимкнено `.pdb`.
- `GetSystemCulture` винесено з бази в `Helpers/SystemCulture.cs`.

**Реліз:** нова іконка у стилі HardwareMonitor (темна плитка, календар, зелений «сьогодні»), `README.md`,
`LICENSE` (MIT), `.github/FUNDING.yml`, версія пакета 1.1, оновлений опис. Структуру репозиторію приведено до
HardwareMonitor: `src/` і `.sln` у корені (без вкладеної папки), `docs/images/` (hero + картинка кожного дизайну),
без `.vscode/` і без CHANGELOG, гілка `main`, `origin` = `github.com/GavryschukOleg/MonthCalendar`.

**Як перевіряли дизайн без пристрою:** окремий консольний проєкт (поза репозиторієм) завантажує
`MonthCalendarPlugin.dll`, для кожного публічного action створює об'єкт без конструктора
(`RuntimeHelpers.GetUninitializedObject`), виставляє `_year/_month/_isDayView` і викликає `GetCommandImage`
для `Width60/90/116`, зберігаючи PNG (`BitmapImage.ToArray(Png)`). Потрібні посилання на DLL з
`C:\Program Files\Logi\LogiPluginService\` (PluginApi, SkiaSharp, Svg.*, ExCSS тощо).

**Подальші кроки:**
- Опублікувати репозиторій і перший реліз `v1.1` (див. «Як випускати»); `supportPageUrl` / `homePageUrl` у yaml вже заповнені.
- Перевірити пакет на macOS.
- Компактніший вигляд місяця для кнопок 50 px, якщо такі пристрої є.
- Ідея: обертання диска для перегортання місяців (потрібен `PluginDynamicAdjustment`).


### 2026-09-17 — сесія: фікс рідкісного «глухого» зависання годинника (потрібен тап)

**Проблема:** інколи (рідко, без відтворюваного сценарію) годинник на кнопці завмирав
і не оновлювався сам по собі — але одразу оживав після тапу по кнопці.

**Причина:** `ScheduleRefresh()` робила `Dispose()` старого `Timer` і створення нового
без синхронізації. Тап користувача (`RunCommand`, головний потік) і спрацювання
попереднього таймера (`OnRefresh` → `finally` → `ScheduleRefresh`, потік пулу) могли
викликати цей метод одночасно. Обидва потоки могли одночасно створити новий `Timer` і
записати його в поле `_refreshTimer` — тоді один із щойно створених таймерів на
мить лишався без жодного посилання (окрім локальної змінної, яку одразу перезаписували) —
достатньо, щоб .NET GC зібрав його ще до першого спрацювання. Таймер зникав без винятку
і без запису в лог, годинник завмирав до наступного тапу (який створював новий таймер
і «випадково» лікував симптом).

**Виправлення (`src/Actions/CalendarCommandBase.cs`):**
- Додано приватний `_timerLock` (`Object`).
- `ScheduleRefresh()`: dispose старого таймера + створення нового обгорнуто в `lock` —
  тепер це атомарна операція, новий `Timer` завжди одразу потрапляє в поле `_refreshTimer`
  і лишається "живим" (rooted), доки його явно не замінять або не задиспозять.
- `OnUnload()`: dispose таймера теж обгорнуто тим самим `lock`, щоб не гонитися з
  паралельним `ScheduleRefresh()`.

**Перевірено:** `dotnet build -c Release` — 0 помилок, 0 попереджень.

**Подальші кроки:**
- Специфічного репро-сценарію немає — стежити, чи зникають повідомлення про завмирання
  протягом наступних тижнів використання.
- Якщо колись повториться — перевірити `MonthCalendar.log` (симптом мав з'являтися без
  жодного запису в лог, на відміну від фіксу 2026-08-14, який завжди логував помилку).

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

З кореня репозиторію (на macOS те саме, з `/` замість `\`):

```powershell
dotnet build -c Release
logiplugintool pack ./bin/Release ./ClockCalendar.lplug4
logiplugintool verify ./ClockCalendar.lplug4
```

`logiplugintool` — глобальний .NET-інструмент (`dotnet tool install --global logiplugintool`).
Збірка також лінкує плагін у Logi Service (`%LOCALAPPDATA%\Logi\LogiPluginService\Plugins\MonthCalendarPlugin.link`
вказує на `bin\Release\`) і перезавантажує його. Після переносу репозиторію в іншу теку потрібно просто
перезібрати — лінк оновиться. Лог плагіна: `%LOCALAPPDATA%\Logi\LogiPluginService\Logs\plugin_logs\MonthCalendar.log`
(має бути рядок `24 dynamic actions loaded`).

## Як випускати

1. Піднести `version` у `src/package/metadata/LoupedeckPackage.yaml`.
2. На Windows: `dotnet build -c Release` → `bin/Release/win/` (там лише `MonthCalendarPlugin.dll` і `.deps.json`).
3. На Mac: потрібна та сама версія коду. Найпростіше — скопіювати проєкт цілком (без `obj/`) разом із готовим
   `bin/Release/win/`, на Маку `dotnet build -c Release` (створює `bin/Release/mac/`; **не** робити `dotnet clean` — він
   видаляє всю `bin/Release`, разом із `win/`), далі спакувати й перевірити.
4. Пакувати там, де є обидві папки: `logiplugintool pack ./bin/Release ./ClockCalendar_<версія>.lplug4`, потім `verify`.
   Файл для релізу назвати `ClockCalendar_<версія з підкресленням>.lplug4` (наприклад `ClockCalendar_1_1.lplug4`; ім'я файлу — назва для користувача, а не технічний `MonthCalendar`) — так
   само, як у HardwareMonitor.
5. На GitHub: тег `v1.1`, заголовок `ClockCalendar 1.1`, опис — рядок-резюме, «What's new in 1.1», «Install»
   (нумеровані кроки) і посилання на README. `.lplug4` у git не комітиться (є в `.gitignore`). Перед релізом перевіряти пакет
   на обох ОС.
