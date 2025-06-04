FROM node:24

WORKDIR /src
COPY package*.json ./
RUN npm install
RUN npm install reactjs-popup
RUN npm install jwt-decode
RUN npm install @mui/material @emotion/react @emotion/styled
RUN npm install --save-dev @swc/core @swc/jest
RUN npm install -g serve
RUN npm fund
COPY . .

RUN npm run build

EXPOSE 80
CMD ["serve", "-s", "dist", "-l", "80"]