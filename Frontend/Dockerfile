# build from source
# ----------------------------
    FROM node:20 AS build

    WORKDIR /app
    
    COPY package*.json .
    RUN npm install
    
    COPY . .
    ARG API_URL
    ENV API_URL $API_URL
    RUN npm run build -- --configuration production
    
    # ----------------------------
    # run with nginx
    # ----------------------------
    FROM nginx
    
    RUN rm /etc/nginx/conf.d/default.conf
    COPY nginx.conf /etc/nginx/conf.d
    COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html