export default {
  testEnvironment: "jest-environment-jsdom",
  transform: {
    "^.+\\.(t|)]sx?$": "@swc/jest"
  },
  moduleNameMapper: {
    "^@/(.*)$": "<rootDir>/src/$1"
  }
};