# Watch Face Designer

A web-based tool for designing smartwatch faces, migrated from a Windows WPF application to a modern React-based web application.

## Tech Stack

- **Frontend**: React 19 + TypeScript
- **State Management**: MobX (MVVM Architecture)
- **Canvas Engine**: Fabric.js
- **UI Library**: Ant Design
- **Build Tool**: Vite
- **Styling**: SCSS/CSS

## Project Structure

```
src/
├── models/          # Data models (TypeScript interfaces)
├── stores/          # MobX stores (ViewModels)
├── components/      # React components (Views)
│   ├── Canvas/      # Canvas-related components
│   ├── Toolbar/     # Design toolbar
│   ├── PropertyPanel/ # Element properties
│   └── Common/      # Shared components
├── services/        # API services
├── utils/           # Utility functions
├── types/           # TypeScript type definitions
└── assets/          # Static assets
    ├── images/      # Images and icons
    └── configs/     # Configuration files
```

## Features

- **Drag & Drop Design**: Visual editor for watch face elements
- **Real-time Preview**: Live preview of watch face designs
- **Element Library**: Pre-built components (time, date, weather, etc.)
- **Multi-device Support**: Different screen sizes and formats
- **Export Options**: Multiple export formats (JSON, BIN, PNG)
- **MVVM Architecture**: Clean separation of concerns

## Development

### Prerequisites
- Node.js 18+
- npm or yarn

### Installation
```bash
npm install
```

### Development
```bash
npm run dev
```

### Build
```bash
npm run build
```

### Testing
```bash
npm run test
```

## Architecture

This application follows the MVVM (Model-View-ViewModel) pattern:

- **Models**: TypeScript interfaces defining data structures
- **Views**: React components for UI presentation
- **ViewModels**: MobX stores managing state and business logic

The architecture ensures:
- Clear separation of concerns
- Reactive data binding
- Testable business logic
- Maintainable codebase