# TestDB Client

Modern web interface for managing software requirements and test cases.

## Features

- 🔐 User authentication (login/register)
- 📋 Requirements management
- 🧪 Test cases management
- 🔗 Requirement-to-test mapping
- 📊 Dashboard with statistics
- 🎨 Beautiful, polished UI

## Installation

```bash
npm install
```

## Running the Client

```bash
# Development mode (with hot reload)
npm start
```

The client runs on port 3000 by default and will open in your browser automatically.

## Building for Production

```bash
npm run build
```

This creates an optimized production build in the `build` folder.

## Configuration

Create a `.env` file to configure the API URL:

```
REACT_APP_API_URL=http://localhost:3001/api
```

If not set, it defaults to `http://localhost:3001/api`.

## Default Credentials

If you initialized the database with default data:
- Username: `admin`
- Password: `admin123`

## Tech Stack

- React 18
- Axios for API calls
- Modern CSS with gradients and animations
- Responsive design

## Pages

1. **Dashboard** - Overview statistics and welcome page
2. **Requirements** - Create, edit, delete, and view requirements
3. **Test Cases** - Create, edit, delete, and view test cases
4. **Mappings** - Link requirements to test cases
