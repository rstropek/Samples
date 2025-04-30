export type Frame = {
  filename: string;
  frame: {
    x: number;
    y: number;
    w: number;
    h: number;
  };
};

type SpritesheetData = {
  frames: Frame[];
};

export class Spritesheet {
  private image?: HTMLImageElement;
  private data?: SpritesheetData;

  constructor(private name: string) {}

  async load(): Promise<void> {
    const [image, response] = await Promise.all([
      this.loadImage(`assets/${this.name}.png`),
      fetch(`assets/${this.name}.json`)
    ]);

    this.image = image;
    this.data = await response.json();
  }

  public getFrames(animationName: string): Frame[] {
    if (!this.data) {
      throw new Error("Spritesheet not loaded");
    }

    function getFrameIndex(frameName: string): number {
      const index = frameName.substring(animationName.length + 1);
      return parseInt(index);
    }

    return this.data.frames
      .filter((frame) => frame.filename.startsWith(`${animationName}_`))
      .sort((a, b) => getFrameIndex(a.filename) - getFrameIndex(b.filename));
  }

  public drawFrame(frame: Frame, ctx: CanvasRenderingContext2D) {
    if (!this.image) {
      throw new Error("Spritesheet not loaded");
    }

    ctx.drawImage(this.image, frame.frame.x, frame.frame.y, frame.frame.w, frame.frame.h, 0, 0, frame.frame.w, frame.frame.h);
  }

  private async loadImage(name: string): Promise<HTMLImageElement> {
    return new Promise<HTMLImageElement>((res) => {
      const image = new Image();
      image.src = name;
      image.onload = () => res(image);
    });
  }
}
