import { DragBindBase } from './DragDataBase';

export class DragBindAnimFrame extends DragBindBase {
    private animImageSource?: string[];
    private repeatCount?: number;
    private frameRate?: number;
    private isRepeat: boolean = false;
    private isPlay: boolean = false;

    public get ImageSource(): string[] | undefined {
        return this.animImageSource;
    }

    public set ImageSource(value: string[] | undefined) {
        if (this.animImageSource !== value) {
            this.animImageSource = value;
            this.notifyPropertyChanged('ImageSource');
        }
    }

    public get RepeatCount(): number | undefined {
        return this.repeatCount;
    }

    public set RepeatCount(value: number | undefined) {
        if (this.repeatCount !== value) {
            this.repeatCount = value;
            this.notifyPropertyChanged('RepeatCount');
        }
    }

    public get FrameRate(): number | undefined {
        return this.frameRate;
    }

    public set FrameRate(value: number | undefined) {
        if (this.frameRate !== value) {
            this.frameRate = value;
            this.notifyPropertyChanged('FrameRate');
        }
    }

    public get IsRepeat(): boolean {
        return this.isRepeat;
    }

    public set IsRepeat(value: boolean) {
        if (this.isRepeat !== value) {
            this.isRepeat = value;
            this.notifyPropertyChanged('IsRepeat');
        }
    }

    public get IsPlay(): boolean {
        return this.isPlay;
    }

    public set IsPlay(value: boolean) {
        if (this.isPlay !== value) {
            this.isPlay = value;
            this.notifyPropertyChanged('IsPlay');
        }
    }

    protected notifyPropertyChanged(propertyName: string): void {
        const handlers = (this as any).propertyChangeHandlers?.get(propertyName);
        if (handlers) {
            handlers.forEach((handler: Function) => handler(this, propertyName));
        }
    }

    public getAllImages(): (string | undefined)[] {
        return this.animImageSource || [];
    }

    public getOutXml(): any {
        const outXml: any = {
            imageArrays: [],
            sprites: [],
            layout: null
        };

        const array: any = {
            name: `anim_array_${Date.now()}`,
            images: this.animImageSource?.map(x => ({ src: x })) || []
        };
        outXml.imageArrays.push(array);

        const sprite: any = {
            name: `sprite_${Date.now()}`,
            ref: `@${array.name}`,
            interval: this.frameRate ? Math.floor(1000 / this.frameRate) : 100,
            repeatCount: this.isRepeat ? 0 : (this.repeatCount || 1),
            imageCount: this.animImageSource?.length || 0
        };

        outXml.sprites.push(sprite);

        outXml.layout = {
            ref: `@${sprite.name}`,
            x: Math.floor(this.left || 0),
            y: Math.floor(this.top || 0)
        };

        return outXml;
    }
}