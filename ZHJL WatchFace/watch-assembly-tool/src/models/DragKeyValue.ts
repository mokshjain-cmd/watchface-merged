import { DragBindBase } from './DragDataBase';
import { DataItemTypeHelper } from './DataItemType';

export interface KeyValueData {
    key?: string;
    value?: string;
}

export class DragBindKeyValue extends DragBindBase {
    private keyValueMaxNum: number = 0;
    private currentNum: number = 0;
    private keyValueImageSource?: string[];
    public keyValues: Map<number, string> = new Map();
    public valueDataImages?: KeyValueData[];

    constructor();
    constructor(keyValues: Map<number, string>, path: string[]);
    constructor(keyValues?: Map<number, string>, path?: string[]) {
        super();
        if (keyValues && path) {
            this.keyValues = keyValues;
            this.keyValueMaxNum = keyValues.size - 1;
            this.minNum = 0;
            if (path.length < keyValues.size) {
                const temp = [...path];
                const diff = keyValues.size - path.length;
                for (let i = 0; i < diff; i++) {
                    temp.push("");
                }
                this.keyValueImageSource = temp;
            } else {
                this.keyValueImageSource = [...path];
            }

            this.valueDataImages = Array.from(keyValues.entries()).map(([key, value], index) => ({
                key: value,
                value: this.keyValueImageSource[index]
            }));
        }
    }

    public get KeyValue(): { key: number; value: string } | undefined {
        if (this.keyValues.size > 0 && this.currentNum <= this.keyValues.size - 1) {
            const entries = Array.from(this.keyValues.entries());
            return { key: entries[this.currentNum][0], value: entries[this.currentNum][1] };
        }
        return undefined;
    }

    public get CurrentNum(): number {
        return this.currentNum;
    }

    public set CurrentNum(value: number) {
        if (this.currentNum !== value) {
            this.currentNum = value;
            this.notifyPropertyChanged('CurrentNum');
            this.notifyPropertyChanged('KeyValue');
        }
    }

    public get ImageSource(): string[] | undefined {
        return this.keyValueImageSource;
    }

    public set ImageSource(value: string[] | undefined) {
        if (this.keyValueImageSource !== value) {
            this.keyValueImageSource = value;
            this.notifyPropertyChanged('ImageSource');
        }
    }

    protected notifyPropertyChanged(propertyName: string): void {
        const handlers = (this as any).propertyChangeHandlers?.get(propertyName);
        if (handlers) {
            handlers.forEach((handler: Function) => handler(this, propertyName));
        }
    }

    public getAllImages(): (string | undefined)[] {
        return this.keyValueImageSource || [];
    }

    private getImageArrayByWeather(): any {
        // This would need weather-specific logic from StaticData
        // For now, return a basic array
        const imageArray = Array.from(this.keyValues.entries())
            .map(([key, value], index) => this.keyValueImageSource?.[index])
            .filter(img => img);

        return {
            name: `weather_array_${Date.now()}`,
            images: imageArray?.map(x => ({ src: x })) || []
        };
    }

    public getOutXml(): any {
        const outXml: any = {
            images: [],
            imageArrays: [],
            dataItemImageValues: [],
            layout: null
        };

        let array: any;
        if (this.itemName === "Weather") {
            array = this.getImageArrayByWeather();
        } else {
            array = {
                name: `array_${Date.now()}`,
                images: this.keyValueImageSource?.map(x => ({ src: x })) || []
            };
        }

        outXml.imageArrays.push(array);

        const dataItem: any = {
            name: `dataItem_${Date.now()}`,
            source: DataItemTypeHelper.DataItemTypes[this.itemName!]?.toString() || '0',
            ref: `@${array.name}`
        };

        if (this.itemName === "Weather") {
            // Weather-specific params would go here
            dataItem.params = Array.from(this.keyValues.entries()).map(([key, value]) => ({
                value: key,
                label: value
            }));
        } else {
            dataItem.params = Array.from(this.keyValues.entries()).map(([key, value]) => ({
                value: key,
                label: value
            }));
        }

        outXml.dataItemImageValues.push(dataItem);

        outXml.layout = {
            ref: `@${dataItem.name}`,
            x: Math.floor(this.left || 0),
            y: Math.floor(this.top || 0)
        };

        return outXml;
    }
}