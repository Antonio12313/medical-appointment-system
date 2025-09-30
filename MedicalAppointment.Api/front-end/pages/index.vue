<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
    <div class="text-center">
      <h1 class="text-5xl font-bold text-gray-800 mb-4">Sistema de Agendamento Médico</h1>
      <p class="text-xl text-gray-600 mb-8">Gerencie seus horários e consultas de forma simples e eficiente</p>

      <div class="flex gap-4 justify-center">
        <NuxtLink
            to="/login"
            class="bg-blue-600 text-white px-8 py-3 rounded-lg font-semibold hover:bg-blue-700 transition-colors"
        >
          Entrar
        </NuxtLink>

        <NuxtLink
            to="/cadastro"
            class="bg-white text-blue-600 px-8 py-3 rounded-lg font-semibold border-2 border-blue-600 hover:bg-blue-50 transition-colors"
        >
          Cadastrar-se
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

onMounted(() => {
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
})
</script>