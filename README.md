# Stellar Myth

A web application that integrates Vue.js with Unity WebGL for an interactive gaming experience on the Stellar blockchain.

## Technologies

- Vue.js 3
- Tailwind CSS
- Firebase (Authentication & Firestore)
- Unity WebGL
- Stellar Soroban (Blockchain)

## Quick Start

1. **Install dependencies:**

   ```bash
   npm install
   ```

2. **Set up environment variables:**

   ```bash
   cp .env.example .env
   ```

   Edit the `.env` file to include your Firebase configuration, if you don't have one, you can create a Firebase project at [Firebase Console](https://console.firebase.google.com/).

3. **Run development server:**

   ```bash
   npm run dev
   ```

4. **Access the application:**
   Open your browser and navigate to `http://localhost:5173`

## Unity Integration (Optional)

A pre-built Unity WebGL build is already included in the `public/webgl` directory.

If you want to use your own Unity build:

1. Compile your Unity project to WebGL
2. Copy the build files to the `public/webgl` directory

## Project Structure

- `src/` - Vue.js application source
- `public/webgl/` - Unity WebGL build
- `project-stellar-myth-unity/` - Unity project source

## License

See the [LICENSE](LICENSE) file for details
