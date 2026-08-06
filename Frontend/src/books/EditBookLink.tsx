import { Link } from "react-router-dom";
import { useCurrentMember } from "../auth/useCurrentMember";

type EditBookLinkProps = {
    bookId: number;
};

export function EditBookLink({ bookId }: EditBookLinkProps) {
    const currentMemberQuery = useCurrentMember();

    // because left side will be false in this if, right side will be checked too
    // also, if CMQ.isSuccess outside the if statement, it will still check role
    if (
        !currentMemberQuery.isSuccess ||
        currentMemberQuery.data.role !== "Administrator"
    ) {
        return null;
    }

    return <Link to={`/books/${bookId}/edit`}>Edit book</Link>;
}