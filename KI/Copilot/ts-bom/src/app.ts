import { Assembly } from "./part-hierarchy";

type Part = {
    id: string,
    name: string,
    quantity: number
}

export function generate_bill_of_material(assembly: Assembly, quantity: number): Part[] {
    const result = new Map<string, Part>();
    for (let part of assembly.parts) {
        if ("parts" in part.component) {
            const subAssemblyParts = generate_bill_of_material(part.component, part.quantity * quantity);
            for (let subPart of subAssemblyParts) {
                const existingPart = result.get(subPart.id);
                if (existingPart) {
                    existingPart.quantity += subPart.quantity;
                } else {
                    result.set(subPart.id, subPart);
                }
            }
        } else {
            const existingPart = result.get(part.component.id);
            if (existingPart) {
                existingPart.quantity += part.quantity * quantity;
            } else {
                result.set(part.component.id, {
                    id: part.component.id,
                    name: part.component.name,
                    quantity: part.quantity * quantity,
                });
            }
        }
    }
    return Array.from(result.values());
}
