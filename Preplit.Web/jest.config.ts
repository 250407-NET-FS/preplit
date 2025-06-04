export default {
    preset: "ts-jest",
    testEnvironment: "jest-environment-jsdom",
    transform: {
        "^.+\\.(t|)]sx?$": "@swc/jest"
    },
    moduleNameMapper: {
        "^@/(.*)$": "<rootDir>/src/$1"
    },
    globals: {
        'ts-jest': {
            diagnostics: true,
            tsconfig: 'tsconfig.json',
        },
    },
    transformIgnorePatterns: ['node_modules/(?!axios|jwt-decode|@testing-library/react)'],    
    moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx', 'node'],
};