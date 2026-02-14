# Web UI Interface for ID/Text Pairs

A modern, polished web interface for displaying and managing ID/Text pairs. Built with React, Vite, and Tailwind CSS.

## Features

- **Modern Design**: High-end UI with gradient backgrounds, glassmorphism effects, and smooth animations
- **Responsive**: Works seamlessly on desktop, tablet, and mobile devices
- **Performance**: Built with Vite for fast development and optimized production builds
- **Tailwind CSS**: Utility-first styling for a consistent, professional look
- **Interactive**: Hover effects, animations, and responsive buttons

## Getting Started

### Prerequisites

- Node.js 18+ and npm

### Installation

```bash
# Install dependencies
npm install
```

### Development

```bash
# Start development server
npm run dev
```

The application will be available at `http://localhost:5173/`

### Build

```bash
# Create production build
npm run build
```

The build output will be in the `dist/` directory.

### Preview Production Build

```bash
# Preview production build locally
npm run preview
```

## Project Structure

```
Client/
├── public/          # Static assets
├── src/
│   ├── assets/      # Images and other assets
│   ├── App.jsx      # Main application component
│   ├── App.css      # Custom animations and styles
│   ├── index.css    # Tailwind CSS directives
│   └── main.jsx     # Application entry point
├── index.html       # HTML template
└── package.json     # Dependencies and scripts
```

## Technology Stack

- **React 19.2**: Modern UI library
- **Vite 7.3**: Next-generation frontend tooling
- **Tailwind CSS v4**: Utility-first CSS framework
- **ESLint**: Code linting and quality

## Customization

To modify the ID/Text pairs data, edit the `items` state in `src/App.jsx`:

```javascript
const [items] = useState([
  { id: 1, text: 'Your text here' },
  { id: 2, text: 'Another item' },
  // Add more items...
])
```

## License

This project is part of the TestDB repository.
