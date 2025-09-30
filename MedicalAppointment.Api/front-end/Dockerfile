FROM node:20-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm install --only=production

COPY . .

RUN npm install

ARG NUXT_PUBLIC_API_BASE
ENV NUXT_PUBLIC_API_BASE=${NUXT_PUBLIC_API_BASE}

RUN npm run build


FROM node:20-alpine AS final

WORKDIR /app

ENV NODE_ENV=production

COPY --from=build /app/.output .

EXPOSE 3000

CMD ["node", "./server/index.mjs"]