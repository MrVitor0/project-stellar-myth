import { createApp } from "vue";
import { createRouter, createWebHistory } from "vue-router";
import App from "./App.vue";
import "./style.css";

// Importar as páginas
import Home from "./views/Home.vue";

// Configurar as rotas
const routes = [{ path: "/", component: Home }];

// Criar o router
const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Criar e montar a aplicação
const app = createApp(App);
app.use(router);
app.mount("#app");
