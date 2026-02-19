export interface Watch {
  name: string;
  vendor: string;
  board: string;
  width?: number | null;
  height?: number | null;
  shape?: string;
  image?: string | null;
}

export interface WatchRoute {
  vendor: string;
  board: string;
  path: string;
  port: number;
}
