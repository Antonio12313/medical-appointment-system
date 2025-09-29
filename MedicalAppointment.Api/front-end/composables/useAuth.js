import { useRuntimeConfig } from "nuxt/app";

export const useAuth = () => {
    const config = useRuntimeConfig()
    const apiBase = config.public.apiBase

    const login = async (credentials) => {
        try {
            return await $fetch(`${apiBase}/api/Auth/login`, {
                method: 'POST',
                body: credentials,
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }
            })
        } catch (error) {
            throw error
        }
    }

    const logout = () => {
        if (typeof window !== 'undefined') {
            localStorage.removeItem('token')
            localStorage.removeItem('usuario')
        }
    }

    const getToken = () => {
        if (typeof window !== 'undefined') {
            return localStorage.getItem('token')
        }
        return null
    }

    const getUser = () => {
        if (typeof window !== 'undefined') {
            const usuario = localStorage.getItem('usuario')
            return usuario ? JSON.parse(usuario) : null
        }
        return null
    }

    return {
        login,
        logout,
        getToken,
        getUser
    }
}