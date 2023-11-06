import {describe, expect, test} from '@jest/globals';
import { generate_bill_of_material } from './app';
import { Assembly } from './part-hierarchy';

describe('generate_bill_of_material', () => {
    test('should return an empty array when given an empty assembly', () => {
        const assembly: Assembly = {
            id: "1",
            name: "Bike",
            parts: [],
        };

        const result = generate_bill_of_material(assembly, 1);

        expect(result).toEqual([]);
    });

    test('should return a single part when given an assembly with a single part', () => {
        const assembly: Assembly = {
            id: "1",
            name: "Bike",
            parts: [
                {
                    component: {
                        id: "2",
                        name: "Frame",
                    },
                    quantity: 1,
                },
            ],
        };

        const result = generate_bill_of_material(assembly, 1);

        expect(result).toEqual([
            { id: "2", name: "Frame", quantity: 1 },
        ]);
    });

    test('should return a single part when given an assembly that contains a single sub-part twice', () => {
        const assembly: Assembly = {
            id: "1",
            name: "Bike",
            parts: [
                {
                    component: {
                        id: "2",
                        name: "Frame",
                    },
                    quantity: 1,
                },
                {
                    component: {
                        id: "2",
                        name: "Frame",
                    },
                    quantity: 1,
                },
            ],
        };

        const result = generate_bill_of_material(assembly, 1);

        expect(result).toEqual([
            { id: "2", name: "Frame", quantity: 2 },
        ]);
    });

    test('should return two parts when given an assembly that contains two different parts', () => {
        const assembly: Assembly = {
            id: "1",
            name: "Bike",
            parts: [
                {
                    component: {
                        id: "2",
                        name: "Frame",
                    },
                    quantity: 1,
                },
                {
                    component: {
                        id: "3",
                        name: "Front Wheel",
                    },
                    quantity: 1,
                },
            ],
        };

        const result = generate_bill_of_material(assembly, 1);

        expect(result).toEqual([
            { id: "2", name: "Frame", quantity: 1 },
            { id: "3", name: "Front Wheel", quantity: 1 },
        ]);
    });

    test('should handle a part that appears in two different sub-assemblies', () => {
        const assembly: Assembly = {
            id: "1",
            name: "Bike",
            parts: [
                {
                    component: {
                        id: "2",
                        name: "Frame",
                    },
                    quantity: 1,
                },
                {
                    component: {
                        id: "3",
                        name: "Front Wheel",
                        parts: [
                            {
                                component: {
                                    id: "4",
                                    name: "Spokes",
                                },
                                quantity: 20,
                            },
                        ],
                    },
                    quantity: 1,
                },
                {
                    component: {
                        id: "4",
                        name: "Spokes",
                    },
                    quantity: 20,
                },
            ],
        };

        const result = generate_bill_of_material(assembly, 1);

        expect(result).toEqual([
            { id: "2", name: "Frame", quantity: 1 },
            { id: "4", name: "Spokes", quantity: 40 },
        ]);
    });

    test('should not return sub-assemblies, only parts', () => {
        const assembly: Assembly = {
            id: "1",
            name: "Bike",
            parts: [
                {
                    component: {
                        id: "3",
                        name: "Front Wheel",
                        parts: [
                            {
                                component: {
                                    id: "4",
                                    name: "Spokes",
                                },
                                quantity: 20,
                            },
                        ],
                    },
                    quantity: 1,
                },
            ],
        };

        const result = generate_bill_of_material(assembly, 1);

        expect(result).toEqual([
            { id: "4", name: "Spokes", quantity: 20 },
        ]);
    })
});