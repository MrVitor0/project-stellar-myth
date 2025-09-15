// Utilitários para o projeto
export const utils = {
  // Formatação de data
  formatDate: (date) => {
    return new Intl.DateTimeFormat("pt-BR", {
      year: "numeric",
      month: "long",
      day: "numeric",
    }).format(new Date(date));
  },

  // Debounce function
  debounce: (func, wait) => {
    let timeout;
    return function executedFunction(...args) {
      const later = () => {
        clearTimeout(timeout);
        func(...args);
      };
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
    };
  },

  // Capitalizar primeira letra
  capitalize: (str) => {
    return str.charAt(0).toUpperCase() + str.slice(1);
  },

  // Gerar cor aleatória
  randomColor: () => {
    const colors = [
      "bg-red-500",
      "bg-blue-500",
      "bg-green-500",
      "bg-yellow-500",
      "bg-purple-500",
      "bg-pink-500",
      "bg-indigo-500",
      "bg-gray-500",
    ];
    return colors[Math.floor(Math.random() * colors.length)];
  },
};

// Constantes do projeto
export const constants = {
  APP_NAME: "Project Stellar Myth",
  VERSION: "1.0.0",
  AUTHOR: "Stellar Development Team",

  // Breakpoints do Tailwind
  BREAKPOINTS: {
    sm: "640px",
    md: "768px",
    lg: "1024px",
    xl: "1280px",
    "2xl": "1536px",
  },

  // Cores do tema
  THEME_COLORS: {
    primary: "stellar",
    secondary: "gray",
    success: "green",
    warning: "yellow",
    error: "red",
  },
};
