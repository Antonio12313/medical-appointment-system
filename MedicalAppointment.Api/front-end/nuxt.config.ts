import {process} from "std-env";

export default defineNuxtConfig({
    compatibilityDate: '2025-07-15',
    devtools: {enabled: true},

    css: ['~/assets/css/main.css'],

    modules: [
        '@nuxtjs/tailwindcss'
    ],

    runtimeConfig: {
        public: {
            apiBase: process.env.API_BASE_URL || 'http://localhost:8080'
        }
    },

    app: {
        head: {
            title: 'Sistema de Agendamento Médico',
            meta: [
                {charset: 'utf-8'},
                {name: 'viewport', content: 'width=device-width, initial-scale=1'},
                {hid: 'description', name: 'description', content: 'Sistema de agendamento médico online'}
            ]
        }
    },

    devServer: {
        https: false,
        port: 3000
    },
    ssr: false,
    nitro: {
        devProxy: {
            '/api': {
                target: process.env.API_BASE_URL || 'http://localhost:8080',
                changeOrigin: true,
                secure: false
            }
        }
    }
})
