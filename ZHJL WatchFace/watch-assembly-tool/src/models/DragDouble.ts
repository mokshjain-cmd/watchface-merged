import { DragBindBase } from './DragDataBase';
import { DataItemTypeHelper } from './DataItemType';

export class DragBindDouble extends DragBindBase {
    private currentNum: number = 0;
    private unitSource?: string;
    private doubleImageSource?: string[];
    private pointSource?: string;
    private emptySource?: string;
    private decimalOffsetX: number = 0;
    private trailingZero: boolean = false;
    public decimalPlaces: number = 1;

    constructor();
    constructor(maxNum: number, minNum: number, current: number);
    constructor(maxNum?: number, minNum?: number, current?: number) {
        super();
        if (maxNum !== undefined && minNum !== undefined && current !== undefined) {
            this.maxNum = maxNum;
            this.minNum = minNum;
            this.currentNum = current;
        }
    }

    public get CurrentNum(): number {
        return Math.round(this.currentNum * Math.pow(10, this.decimalPlaces)) / Math.pow(10, this.decimalPlaces);
    }

    public set CurrentNum(value: number) {
        if (this.currentNum !== value) {
            this.currentNum = value;
            this.notifyPropertyChanged('CurrentNum');
        }
    }

    public get UnitSource(): string | undefined {
        return this.unitSource;
    }

    public set UnitSource(value: string | undefined) {
        if (this.unitSource !== value) {
            this.unitSource = value;
            this.notifyPropertyChanged('UnitSource');
        }
    }

    public get ImageSource(): string[] | undefined {
        return this.doubleImageSource;
    }

    public set ImageSource(value: string[] | undefined) {
        if (this.doubleImageSource !== value) {
            this.doubleImageSource = value;
            this.notifyPropertyChanged('ImageSource');
        }
    }

    public get PointSource(): string | undefined {
        return this.pointSource;
    }

    public set PointSource(value: string | undefined) {
        if (this.pointSource !== value) {
            this.pointSource = value;
            this.notifyPropertyChanged('PointSource');
        }
    }

    public get EmptySource(): string | undefined {
        return this.emptySource;
    }

    public set EmptySource(value: string | undefined) {
        if (this.emptySource !== value) {
            this.emptySource = value;
            this.notifyPropertyChanged('EmptySource');
        }
    }

    public get DecimalOffsetX(): number {
        return this.decimalOffsetX;
    }

    public set DecimalOffsetX(value: number) {
        if (this.decimalOffsetX !== value) {
            this.decimalOffsetX = value;
            this.notifyPropertyChanged('DecimalOffsetX');
        }
    }

    public get TrailingZero(): boolean {
        return this.trailingZero;
    }

    public set TrailingZero(value: boolean) {
        if (this.trailingZero !== value) {
            this.trailingZero = value;
            this.notifyPropertyChanged('TrailingZero');
        }
    }

    protected notifyPropertyChanged(propertyName: string): void {
        // Implementation from base class pattern
        const handlers = (this as any).propertyChangeHandlers?.get(propertyName);
        if (handlers) {
            handlers.forEach((handler: Function) => handler(this, propertyName));
        }
    }

    public getAllImages(): (string | undefined)[] {
        const images: (string | undefined)[] = [];
        if (this.doubleImageSource && this.doubleImageSource.length > 0) {
            images.push(...this.doubleImageSource);
        }
        images.push(this.unitSource);
        images.push(this.pointSource);
        images.push(this.emptySource);
        return images;
    }

    private getNumStr(trailingZero: boolean, currentNum: number, decimalPlaces: number): string {
        if (trailingZero) {
            return currentNum.toFixed(decimalPlaces);
        } else {
            return currentNum.toFixed(1).replace(/\.?0+$/, '');
        }
    }

    public getOutXml(): any {
        if (this.verifyNullNum && this.doubleImageSource && this.doubleImageSource.length !== 11) {
            throw new Error(`${this.dragName}的图片数量不正确，请检查是否添加无数据图片`);
        }

        const outXml: any = {
            images: [],
            imageArrays: [],
            dataItemImageNumbers: [],
            layout: null
        };

        let unitIcon: any = null;
        if (this.unitSource) {
            unitIcon = {
                src: this.unitSource,
                name: `unit_${Date.now()}`
            };
            outXml.images.push(unitIcon);
        }

        const array: any = {
            name: `array_${Date.now()}`,
            images: this.doubleImageSource?.map(x => ({ src: x })) || []
        };

        if (this.emptySource) {
            array.images.push({ src: this.emptySource });
        }

        if (this.pointSource) {
            array.images.push({ src: this.pointSource });
        }

        outXml.imageArrays.push(array);

        const dataItem: any = {
            name: `dataItem_${Date.now()}`,
            source: DataItemTypeHelper.DataItemTypes[this.itemName!]?.toString() || '0',
            ref: `@${array.name}`,
            unitIcon: unitIcon ? `@${unitIcon.name}` : "",
            align: this.align.toString(),
            totalDigits: 5, // Default for double values
            decimalDigits: this.itemName === "Step Distance" ? 2 : 1,
            decimalOffsetX: this.decimalOffsetX,
            trailingZero: this.trailingZero,
            renderRule: "alwaysShow"
        };

        outXml.dataItemImageNumbers.push(dataItem);

        outXml.layout = {
            ref: `@${dataItem.name}`,
            x: Math.floor(this.left || 0),
            y: Math.floor(this.top || 0)
        };

        return outXml;
    }
}