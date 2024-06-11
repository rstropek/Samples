export type Part = {
    id: string;
    name: string;
}

export type PartOrAssembly = Part | Assembly;

export type Assembly = {
    id: string;
    name: string;
    parts: {
        component: PartOrAssembly;
        quantity: number;
    }[];
}
