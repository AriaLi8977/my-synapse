import type { Note } from "../types/note";
import { Link } from "react-router-dom";

interface Props{ 
    note: Note;
    onDelete: (id: string) => void;
}

export function NoteCard({ note, onDelete }: Props) {

    const statusMap: Record<number, {text: string; color:string}>={
        0:{
            text: "Pending",
            color: "bg-yellow-100 text-yellow-800"
        },
        1:{
            text: "Processing",
            color: "bg-blue-100 text-blue-800"
        },
        2:{
            text: "Completed",
            color: "bg-green-100 text-green-800"
        },
        3:{
            text: "Failed",
            color: "bg-red-100 text-red-800"
        }
    }

    const statusInfo = statusMap[Number(note.status)] || {text: "Unknown", color: "bg-gray-100 text-gray-800"};

    
    return (
      <div className="bg-white rounded-xl shadow p-5">
  
        <div className="flex justify-between items-center">
  
          <h3 className="font-semibold text-lg">
           Note Title: <Link to={`/notes/${note.id}`} className="text-blue-600 hover:underline">{note.title}</Link>
          </h3>
  
          <span className={`text-sm rounded-full font-medium ${statusInfo.color} px-2 py-1`}>
            {statusInfo.text}
          </span>
        </div>
  
        <p className="mt-4 text-gray-700 whitespace-pre-wrap">
          {note.content}
        </p>
  
        {note.summary && (
          <>
            <hr className="my-4" />
  
            <h4 className="font-medium mb-2">
              AI Summary
            </h4>
  
            <p className="text-gray-700">
              {note.summary}
            </p>

<div className="flex justify-between items-center mt-4 text-gray-500 text-sm">
    <span>
        {new Date(note.createdAt).toLocaleString()}
    </span>
    <button
        onClick={() => {
            if (window.confirm("Are you sure you want to delete this note?")) {
                onDelete(note.id);
            }
        }}
        className="bg-white text-red-500 px-2 py-2 rounded hover:bg-gray-100"
        aria-label="Delete"
    >
        <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-4 w-4"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            strokeWidth={2}
        >
            <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M19 7l-.867 12.142A2 2 0 0116.134 21H7.866a2 2 0 01-1.999-1.858L5 7m5-4h4m-4 0a2 2 0 00-2 2v1h8V5a2 2 0 00-2-2m-4 0h4"
            />
        </svg>
    </button>
</div>
            </>
            )}
    
        </div>
    );
  } 