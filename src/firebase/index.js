import { initializeApp } from "firebase/app";
import { getAuth } from "firebase/auth";
import { getFirestore } from "firebase/firestore";

// Verificar se as variáveis de ambiente estão definidas
const requiredEnvVars = [
  "VITE_FIREBASE_API_KEY",
  "VITE_FIREBASE_AUTH_DOMAIN",
  "VITE_FIREBASE_PROJECT_ID",
  "VITE_FIREBASE_STORAGE_BUCKET",
  "VITE_FIREBASE_MESSAGING_SENDER_ID",
  "VITE_FIREBASE_APP_ID",
];

const missingEnvVars = requiredEnvVars.filter(
  (varName) => !import.meta.env[varName]
);
if (missingEnvVars.length > 0) {
  console.error(
    `Erro: Variáveis de ambiente faltando: ${missingEnvVars.join(", ")}`
  );
  console.error("Por favor, crie um arquivo .env baseado no .env.example");
}

// Configuração do Firebase usando variáveis de ambiente
const firebaseConfig = {
  apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
  projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
  storageBucket: import.meta.env.VITE_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER_ID,
  appId: import.meta.env.VITE_FIREBASE_APP_ID,
  measurementId: import.meta.env.VITE_FIREBASE_MEASUREMENT_ID,
};

// Inicialização do Firebase
const app = initializeApp(firebaseConfig);

// Serviços Firebase
const auth = getAuth(app);
const db = getFirestore(app);

export { auth, db };
