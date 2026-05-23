/** @type {import('ts-jest').JestConfigWithTsJest} **/
export default {
  testEnvironment: "node",
  transform: {
    "^.+\\.(tsx?|ts)?$": ["ts-jest", {}],
  },
};
