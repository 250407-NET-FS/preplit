import type { Config } from '@jest/types';

const config: Config.InitialOptions = {
    verbose: true,
    preset: "ts-jest",
    testEnvironment: "jest-environment-jsdom",
    transform: {
        "^.+\\.(t|j)]sx?$": ["ts-jest", {
            diagnostics: true,
            tsconfig: './tsconfig.json',
        }]
    },
    moduleNameMapper: {
        "^@/(.*)$": "<rootDir>/src/$1"
    },
    transformIgnorePatterns: ['node_modules/(?!axios|jwt-decode|@testing-library/react)'],    
    moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx', 'node'],
};

export default config;