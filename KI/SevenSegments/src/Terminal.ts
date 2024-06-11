export class Terminal {
  clearScreen() {
    process.stdout.write("\x1Bc");
  }

  write(s: string) {
    process.stdout.write(s);
  }

  writeNewline() {
    process.stdout.write('\n');
  }
}
