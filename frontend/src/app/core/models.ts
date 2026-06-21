export interface User {
  id: number;
  fullName: string;
  email: string;
  role: 'Admin' | 'Customer';
}

export interface AuthSession {
  token: string;
  user: User;
}

export interface Category {
  id: number;
  name: string;
}

export interface ServiceOffering {
  id: number;
  categoryId: number;
  categoryName: string;
  name: string;
  description: string;
  durationMinutes: number;
  price: number;
  imageUrl: string;
  isActive: boolean;
}

export interface ServiceInput {
  categoryId: number;
  name: string;
  description: string;
  durationMinutes: number;
  price: number;
  imageUrl: string;
  isActive: boolean;
}

export interface CartItem {
  id: number;
  userId: number;
  serviceId: number;
  serviceName: string;
  durationMinutes: number;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface OrderItem {
  id: number;
  orderId: number;
  serviceId: number;
  serviceName: string;
  durationMinutes: number;
  unitPrice: number;
  quantity: number;
}

export interface Order {
  id: number;
  userId: number;
  customerName: string;
  customerEmail: string;
  bookingDate: string;
  notes: string;
  status: 'Pending' | 'Confirmed' | 'Paid' | 'Completed' | 'Cancelled';
  totalAmount: number;
  createdAt: string;
  items: OrderItem[];
}

export interface DashboardSummary {
  userCount: number;
  activeServiceCount: number;
  orderCount: number;
  revenue: number;
}

export interface ApiError {
  message: string;
}
