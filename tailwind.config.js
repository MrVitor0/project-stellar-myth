/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{vue,js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        // Gaming & Blockchain Theme Colors with Brazilian Dark Inspiration
        "dark-night": "#0D1117", // Fundo principal escuro
        "slate-concrete": "#1A1E23", // Chumbo-Calçada mais escuro
        "twilight-blue": "#1C2541", // Azul-Crepúsculo (cards)
        "brazil-green": "#009739", // Verde Brasil
        "brazil-yellow": "#FFCD00", // Amarelo Brasil
        "mist-white": "#E1E1E1", // Branco-Névoa (títulos)
        "mid-gray": "#8D99AE", // Cinza-Intermediário (texto)
        "lamp-yellow": "#F2B705", // Amarelo-Lampião (botões)
        "night-forest": "#0B5351", // Verde-Mata Noturna (links)
        "mystic-cyan": "#40E0D0", // Ciano-Místico (notificações)
        "deep-blue": "#003366", // Azul escuro brasileiro
      },
    },
  },
  plugins: [],
};
