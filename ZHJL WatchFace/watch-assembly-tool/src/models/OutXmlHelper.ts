/**
 * XML output helper utilities equivalent to C# OutXmlHelper
 * Provides methods for generating unique element names and XML output
 */

export class OutXmlHelper {
  private static _allNames: string[] = [];

  /**
   * Get all used names
   */
  public static get allNames(): string[] {
    return this._allNames;
  }

  /**
   * Generate a unique watch element name
   */
  public static getWatchElementName(): string {
    let name = this.generateRandomData(6);
    while (this._allNames.includes(name)) {
      name = this.generateRandomData(6);
    }
    this._allNames.push(name);
    return name;
  }

  /**
   * Get watch element name with prefix
   */
  public static getWatchElementNameByPx(px: string): string {
    const matchingNames = this._allNames
      .filter(x => x.startsWith(px))
      .map(x => parseInt(x.replace(px, '')))
      .filter(x => !isNaN(x));

    const last = matchingNames.length > 0 ? Math.max(...matchingNames) : 0;
    const name = `${px}${last + 1}`;
    this._allNames.push(name);
    return name;
  }

  /**
   * Generate random string data
   */
  public static generateRandomData(length: number = 6): string {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
      result += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return result;
  }

  /**
   * Clear all names (useful for resetting between operations)
   */
  public static clearNames(): void {
    this._allNames = [];
  }

  /**
   * Check if name is already used
   */
  public static isNameUsed(name: string): boolean {
    return this._allNames.includes(name);
  }

  /**
   * Add name to used names list
   */
  public static addName(name: string): void {
    if (!this._allNames.includes(name)) {
      this._allNames.push(name);
    }
  }

  /**
   * Remove name from used names list
   */
  public static removeName(name: string): void {
    const index = this._allNames.indexOf(name);
    if (index > -1) {
      this._allNames.splice(index, 1);
    }
  }
}