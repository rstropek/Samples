import * as needle from 'needle';

interface IForm {
    name: string;
}

interface IPokemon {
    forms: IForm[];
}

function sleep(seconds: number) : Promise<void> {
    return new Promise<void>((resolve, reject) => 
        setTimeout(() => resolve(), seconds * 1000));
}

function getPokemonName(pokemonId: number): Promise<string> {
    return new Promise<string>((resolve, reject) => {
        needle.get(`https://pokeapi.co/api/v2/pokemon/${pokemonId}/`, (err, res) => {
            if (!err && res.statusCode == 200) {
                let pokemon = <IPokemon>res.body;
                resolve(pokemon.forms[0].name);
            } else {
                reject("Could not get pokemon data");
            }
        });
    });
}

async function run() : Promise<void> {
    try {
        var name = await getPokemonName(25);
        console.log(`Pokemon ${25} is ${name}.`);

        // The following line will produce an exception
        name = await getPokemonName(99999);
    }
    catch(ex) {
        console.log(`Error "${ex}" happened.`);
    }
}

run();

/*
 * Note that run does NOT block the execution of the program.
 * Therefore, the following lines start another async. operation
 * in parallel. As a result the program will print something like:
   Heartbeat...
   Heartbeat...
   Heartbeat...
   Pokemon 25 is pikachu.
   Heartbeat...
   Error "Could not get pokemon data" happened.
   Heartbeat...
 */ 
var cb = () => { console.log("Heartbeat..."); setTimeout(cb, 1000); };
cb();
