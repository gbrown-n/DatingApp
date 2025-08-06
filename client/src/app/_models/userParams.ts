import { User } from "./user";

export class UserParams {
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    pageSize = 5;
    gender: string;
    orderBy = 'lastActive';

    constructor(user: User | null) {
        this.gender = user?.gender === 'female' ? 'male' : 'female';
    }
}