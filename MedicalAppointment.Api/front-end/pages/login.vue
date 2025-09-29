<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
    <div class="bg-white rounded-2xl shadow-xl w-full max-w-md p-8">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-gray-800 mb-2">Sistema Médico</h1>
        <p class="text-gray-600">Faça login em sua conta</p>
      </div>

      <div class="mb-6">
        <label class="block text-sm font-medium text-gray-700 mb-3">Entrar como:</label>
        <div class="grid grid-cols-2 gap-3">
          <button
              @click="tipoLogin = 'paciente'"
              :class="[
              'py-3 px-4 rounded-lg border-2 transition-all font-medium',
              tipoLogin === 'paciente'
                ? 'border-blue-500 bg-blue-50 text-blue-700'
                : 'border-gray-300 hover:border-gray-400 text-gray-700'
            ]"
          >
            👤 Paciente
          </button>
          <button
              @click="tipoLogin = 'medico'"
              :class="[
              'py-3 px-4 rounded-lg border-2 transition-all font-medium',
              tipoLogin === 'medico'
                ? 'border-blue-500 bg-blue-50 text-blue-700'
                : 'border-gray-300 hover:border-gray-400 text-gray-700'
            ]"
          >
            👨‍⚕️ Médico
          </button>
        </div>
      </div>

      <form @submit.prevent="handleLogin" class="space-y-6">
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700 mb-2">
            Email
          </label>
          <input
              v-model="loginData.email"
              type="email"
              id="email"
              required
              autocomplete="email"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
              placeholder="seu@email.com"
          />
        </div>

        <div>
          <label for="password" class="block text-sm font-medium text-gray-700 mb-2">
            Senha
          </label>
          <input
              v-model="loginData.senha"
              type="password"
              id="password"
              required
              autocomplete="current-password"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
              placeholder="••••••••"
          />
        </div>

        <div v-if="error" class="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-lg">
          {{ error }}
        </div>

        <button
            type="submit"
            :disabled="loading || !tipoLogin"
            class="w-full bg-blue-600 text-white py-3 px-4 rounded-lg font-semibold hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ loading ? 'Entrando...' : 'Entrar' }}
        </button>
      </form>

      <div class="mt-6 text-center">
        <p class="text-gray-600">
          Não tem uma conta?
          <NuxtLink to="/cadastro" class="text-blue-600 hover:text-blue-700 font-semibold">
            Cadastre-se
          </NuxtLink>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import {onMounted, ref} from 'vue'
import {useRouter} from 'vue-router'
import {process} from "std-env"
import {useRuntimeConfig} from "nuxt/app"

const router = useRouter()
const apiBase = useRuntimeConfig().public.apiBase

const loading = ref(false)
const error = ref('')
const tipoLogin = ref('paciente')

const loginData = ref({
  email: '',
  senha: ''
})

onMounted(() => {
  if (process.client) {
    const token = localStorage.getItem('token')
    const usuario = localStorage.getItem('usuario')

    if (token && usuario) {
      const user = JSON.parse(usuario)
      if (user.tipoUsuario === 'medico') {
        router.push('/medico/dashboard')
      } else if (user.tipoUsuario === 'paciente') {
        router.push('/paciente/dashboard')
      }
    }
  }
})

const handleLogin = async () => {
  loading.value = true
  error.value = ''

  try {
    const response = await $fetch(`${apiBase}/api/Auth/login`, {
      method: 'POST',
      body: loginData.value,
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      }
    })
    console.log('Resposta da API:', response)

    if (response?.token && response?.usuario) {
      if (response.usuario.tipoUsuario !== tipoLogin.value) { 
        error.value = `Esta conta é de ${response.usuario.tipoUsuario}. Selecione o tipo correto.`
        return
      }

      localStorage.setItem('token', response.token)  
      localStorage.setItem('usuario', JSON.stringify(response.usuario))  

      if (response.usuario.tipoUsuario === 'medico') {  
        await router.push('/medico/dashboard')
      } else if (response.usuario.tipoUsuario === 'paciente') {  
        await router.push('/paciente/dashboard')
      }
    } else {
      throw new Error('Resposta inválida do servidor')
    }

  } catch (err) {
    console.error('Erro completo:', err)

    if (err.data?.message) {
      error.value = err.data.message
    } else if (err.response) {
      error.value = `Erro HTTP: ${err.response.status}`
    } else {
      error.value = 'Erro ao fazer login. Verifique suas credenciais.'
    }
  } finally {
    loading.value = false
  }
}
</script>