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
            apiBase: '/api'
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
                target: 'http://medical_api:8080',
                changeOrigin: true
            }
        }
    },

    routeRules: {
        '/api/**': {
            proxy: {
                to: 'http://medical_api:8080/api/**'
            }
        }
    }
})