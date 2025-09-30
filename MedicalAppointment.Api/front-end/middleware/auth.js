import { defineNuxtRouteMiddleware, navigateTo } from "#app";

export default defineNuxtRouteMiddleware((to) => {
    if (typeof window !== 'undefined') {
        const token = localStorage.getItem('token')
        const usuario = localStorage.getItem('usuario')
        
        if (!token || !usuario) {
            if (to.path !== '/login') {
                return navigateTo('/login')
            }
            return
        }

        try {
            const user = JSON.parse(usuario)

            if (to.path.startsWith('/medico') && user.tipoUsuario !== 'medico') {
                return navigateTo('/login')
            }

            if (to.path.startsWith('/paciente') && user.tipoUsuario !== 'paciente') {
                return navigateTo('/login')
            }


        } catch (error) {
            console.error('Erro ao parsear usuário:', error)
            localStorage.removeItem('token')
            localStorage.removeItem('usuario')
            return navigateTo('/login')
        }
    }
})