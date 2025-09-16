import { createApp } from "vue";
import { createRouter, createWebHistory } from "vue-router";
import App from "./App.vue";
import "./style.css";
import authGuard from "./router/authGuard";

// Importar as páginas
import Home from "./views/Home.vue";
import Login from "./views/Login.vue";
import Community from "./views/Community.vue";

// Configurar as rotas
const routes = [
  { path: "/", component: Home },
  { path: "/login", component: Login },
  {
    path: "/community",
    component: Community,
    meta: { requiresAuth: true }, // Esta rota requer autenticação
  },
];

// Criar o router
const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Aplicar o guarda de autenticação a cada navegação
router.beforeEach(authGuard);

// Criar e montar a aplicação
const app = createApp(App);
app.use(router);
app.mount("#app");
