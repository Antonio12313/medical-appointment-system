<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
    <div class="bg-white rounded-2xl shadow-xl w-full max-w-2xl p-8">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-gray-800 mb-2">Criar Conta</h1>
        <p class="text-gray-600">Escolha o tipo de conta e preencha seus dados</p>
      </div>

      <div class="flex gap-4 mb-8">
        <button
            @click="tipoUsuario = 'paciente'"
            :class="[
            'flex-1 py-4 px-6 rounded-lg border-2 transition-all',
            tipoUsuario === 'paciente'
              ? 'border-blue-500 bg-blue-50 text-blue-700'
              : 'border-gray-300 hover:border-gray-400'
          ]"
        >
          <div class="text-lg font-semibold">Sou Paciente</div>
          <div class="text-sm mt-1 opacity-75">Quero agendar consultas</div>
        </button>

        <button
            @click="tipoUsuario = 'medico'"
            :class="[
            'flex-1 py-4 px-6 rounded-lg border-2 transition-all',
            tipoUsuario === 'medico'
              ? 'border-blue-500 bg-blue-50 text-blue-700'
              : 'border-gray-300 hover:border-gray-400'
          ]"
        >
          <div class="text-lg font-semibold">Sou Médico</div>
          <div class="text-sm mt-1 opacity-75">Quero gerenciar meus horários</div>
        </button>
      </div>

      <form @submit.prevent="handleCadastro" class="space-y-6">
        <!-- Dados básicos -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label for="nome" class="block text-sm font-medium text-gray-700 mb-2">
              Nome Completo
            </label>
            <input
                v-model="formData.nome"
                type="text"
                id="nome"
                required
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                placeholder="João Silva"
            />
          </div>

          <div>
            <label for="email" class="block text-sm font-medium text-gray-700 mb-2">
              Email
            </label>
            <input
                v-model="formData.email"
                type="email"
                id="email"
                required
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                placeholder="seu@email.com"
            />
          </div>
        </div>

        <div>
          <label for="senha" class="block text-sm font-medium text-gray-700 mb-2">
            Senha
          </label>
          <input
              v-model="formData.senha"
              type="password"
              id="senha"
              required
              minlength="6"
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
              placeholder="Mínimo 6 caracteres"
          />
        </div>

        <!-- Campos específicos para Paciente -->
        <div v-if="tipoUsuario === 'paciente'" class="space-y-4">
          <h3 class="text-lg font-semibold text-gray-700 mt-6 mb-4">Dados do Paciente</h3>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label for="cpf" class="block text-sm font-medium text-gray-700 mb-2">
                CPF
              </label>
              <input
                  v-model="dadosPaciente.cpf"
                  type="text"
                  id="cpf"
                  required
                  maxlength="14"
                  @input="formatCPF"
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                  placeholder="123.456.789-00"
              />
            </div>

            <div>
              <label for="telefone" class="block text-sm font-medium text-gray-700 mb-2">
                Telefone
              </label>
              <input
                  v-model="dadosPaciente.telefone"
                  type="tel"
                  id="telefone"
                  required
                  maxlength="15"
                  @input="formatTelefone"
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                  placeholder="(11) 98765-4321"
              />
            </div>
          </div>
        </div>

        <!-- Campos específicos para Médico -->
        <div v-if="tipoUsuario === 'medico'" class="space-y-4">
          <h3 class="text-lg font-semibold text-gray-700 mt-6 mb-4">Dados do Médico</h3>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label for="crm" class="block text-sm font-medium text-gray-700 mb-2">
                CRM
              </label>
              <input
                  v-model="dadosMedico.crm"
                  type="text"
                  id="crm"
                  required
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                  placeholder="CRM/SP 123456"
              />
            </div>

            <div>
              <label for="especialidade" class="block text-sm font-medium text-gray-700 mb-2">
                Especialidade
              </label>
              <select
                  v-model="dadosMedico.especialidade"
                  id="especialidade"
                  required
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
              >
                <option value="">Selecione uma especialidade</option>
                <option value="Cardiologia">Cardiologia</option>
                <option value="Dermatologia">Dermatologia</option>
                <option value="Endocrinologia">Endocrinologia</option>
                <option value="Gastroenterologia">Gastroenterologia</option>
                <option value="Ginecologia">Ginecologia</option>
                <option value="Neurologia">Neurologia</option>
                <option value="Oftalmologia">Oftalmologia</option>
                <option value="Ortopedia">Ortopedia</option>
                <option value="Pediatria">Pediatria</option>
                <option value="Psiquiatria">Psiquiatria</option>
                <option value="Urologia">Urologia</option>
                <option value="Clínica Geral">Clínica Geral</option>
              </select>
            </div>
          </div>
        </div>

        <div v-if="error" class="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-lg">
          {{ error }}
        </div>

        <div v-if="success" class="bg-green-50 border border-green-200 text-green-600 px-4 py-3 rounded-lg">
          Cadastro realizado com sucesso! Redirecionando...
        </div>

        <button
            type="submit"
            :disabled="loading || !tipoUsuario"
            class="w-full bg-blue-600 text-white py-3 px-4 rounded-lg font-semibold hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ loading ? 'Cadastrando...' : 'Cadastrar' }}
        </button>
      </form>

      <div class="mt-6 text-center">
        <p class="text-gray-600">
          Já tem uma conta?
          <NuxtLink to="/login" class="text-blue-600 hover:text-blue-700 font-semibold">
            Faça login
          </NuxtLink>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import {useRuntimeConfig} from "nuxt/app";

const router = useRouter()
const loading = ref(false)
const error = ref('')
const success = ref(false)
const tipoUsuario = ref('')

const formData = ref({
  nome: '',
  email: '',
  senha: ''
})

const dadosPaciente = ref({
  cpf: '',
  telefone: ''
})

const dadosMedico = ref({
  crm: '',
  especialidade: ''
})

const formatCPF = (event) => {
  let value = event.target.value.replace(/\D/g, '')
  if (value.length <= 11) {
    value = value.replace(/(\d{3})(\d)/, '$1.$2')
    value = value.replace(/(\d{3})(\d)/, '$1.$2')
    value = value.replace(/(\d{3})(\d{1,2})$/, '$1-$2')
    dadosPaciente.value.cpf = value
  }
}

const formatTelefone = (event) => {
  let value = event.target.value.replace(/\D/g, '')
  if (value.length <= 11) {
    value = value.replace(/^(\d{2})(\d)/g, '($1) $2')
    value = value.replace(/(\d)(\d{4})$/, '$1-$2')
    dadosPaciente.value.telefone = value
  }
}

const handleCadastro = async () => {
  loading.value = true
  error.value = ''

  try {
    const payload = {
      ...formData.value,
      tipoUsuario: tipoUsuario.value,
      dadosMedico: tipoUsuario.value === 'medico' ? dadosMedico.value : null,
      dadosPaciente: tipoUsuario.value === 'paciente' ? dadosPaciente.value : null
    }
    
    const apiBase = useRuntimeConfig().public.apiBase;

    const response = await $fetch(`${apiBase}/api/auth/register`, {
      method: 'POST',
      body: payload,
      headers: {
        'Content-Type': 'application/json'
      }
    })

    localStorage.setItem('token', response.token)
    localStorage.setItem('usuario', JSON.stringify(response.usuario))

    success.value = true

    setTimeout(() => {
      if (response.usuario.tipoUsuario === 'medico') {
        router.push('/medico/dashboard')
      } else if (response.usuario.tipoUsuario === 'paciente') {
        router.push('/paciente/dashboard')
      }
    }, 2000)
  } catch (err) {
    error.value = err.data?.message || 'Erro ao realizar cadastro. Verifique os dados informados.'
  } finally {
    loading.value = false
  }
}
</script>