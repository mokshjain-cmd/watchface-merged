import { DragBindBase } from './DragDataBase';
import { DataItemTypeHelper } from './DataItemType';

export class DragBindPoint extends DragBindBase {
    private startAngle: number = 0;
    private endAngle: number = 360;
    private originPointX: number = 0;
    private originPointY: number = 0;
    private pointSource?: string;
    private pointValue: number = 0;
    private pointAngle: number = 0;
    private valueIndex: number = 0;

    constructor(source?: string) {
        super();
        if (source) {
            // In a real implementation, you'd load the bitmap to get dimensions
            // For now, we'll set default values
            this.pointSource = source;
            this.originPointX = 25; // Half of typical 50px width
            this.originPointY = 25; // Half of typical 50px height
        }
    }

    public get StartAngle(): number {
        return this.startAngle;
    }

    public set StartAngle(value: number) {
        if (this.startAngle !== value) {
            this.startAngle = value;
            this.notifyPropertyChanged('StartAngle');
        }
    }

    public get EndAngle(): number {
        return this.endAngle;
    }

    public set EndAngle(value: number) {
        if (this.endAngle !== value) {
            this.endAngle = value;
            this.notifyPropertyChanged('EndAngle');
        }
    }

    public get OriginPointX(): number {
        return this.originPointX;
    }

    public set OriginPointX(value: number) {
        if (this.originPointX !== value) {
            this.originPointX = value;
            this.notifyPropertyChanged('OriginPointX');
        }
    }

    public get OriginPointY(): number {
        return this.originPointY;
    }

    public set OriginPointY(value: number) {
        if (this.originPointY !== value) {
            this.originPointY = value;
            this.notifyPropertyChanged('OriginPointY');
        }
    }

    public get Source(): string | undefined {
        return this.pointSource;
    }

    public set Source(value: string | undefined) {
        if (this.pointSource !== value) {
            this.pointSource = value;
            this.notifyPropertyChanged('Source');
        }
    }

    public get Value(): number {
        return this.pointValue;
    }

    public set Value(value: number) {
        if (this.pointValue !== value) {
            this.pointValue = value;
            this.notifyPropertyChanged('Value');
        }
    }

    public get Angle(): number {
        return this.pointAngle;
    }

    public set Angle(value: number) {
        if (this.pointAngle !== value) {
            this.pointAngle = value;
            this.notifyPropertyChanged('Angle');
        }
    }

    public get ValueIndex(): number {
        return this.valueIndex;
    }

    public set ValueIndex(value: number) {
        if (this.valueIndex !== value) {
            this.valueIndex = value;
            this.endAngle = value === 0 ? 720 : 360;
            this.notifyPropertyChanged('ValueIndex');
        }
    }

    protected notifyPropertyChanged(propertyName: string): void {
        const handlers = (this as any).propertyChangeHandlers?.get(propertyName);
        if (handlers) {
            handlers.forEach((handler: Function) => handler(this, propertyName));
        }
    }

    public getAllImages(): (string | undefined)[] {
        return [this.pointSource];
    }

    public getOutXml(): any {
        const outXml: any = {
            images: [],
            dataItemPointers: [],
            layout: null
        };

        const image: any = {
            name: `pointer_${Date.now()}`,
            src: this.pointSource,
            isPointer: true
        };
        outXml.images.push(image);

        const dataItem: any = {
            name: `pointer_data_${Date.now()}`,
            source: this.valueIndex === 0 ?
                DataItemTypeHelper.DataItemTypes["Hour"]?.toString() :
                this.valueIndex === 1 ?
                    DataItemTypeHelper.DataItemTypes["Minute"]?.toString() :
                    DataItemTypeHelper.DataItemTypes["Second"]?.toString(),
            ref: `@${image.name}`,
            angleStart: 0,
            angleRange: this.valueIndex === 0 ? 720 : 360,
            valueStart: 0,
            valueRange: this.valueIndex === 0 ? 24 : 60,
            pivotX: this.originPointX,
            pivotY: this.originPointY
        };

        outXml.dataItemPointers.push(dataItem);

        outXml.layout = {
            ref: `@${dataItem.name}`,
            x: Math.floor(this.left || 0),
            y: Math.floor(this.top || 0)
        };

        return outXml;
    }
}