import type { Note } from "../types/note";

interface Props{ note: Note;}

export function NoteCard({ note }: Props) {
    return (
      <div className="bg-white rounded-xl shadow p-5">
  
        <div className="flex justify-between items-center">
  
          <h3 className="font-semibold text-lg">
            Note
          </h3>
  
          <span className="text-sm text-gray-500">
          <div>
            {note.status}
            {note.status === "Pending" && "⏳ Pending"}
            {note.status === "Processing" && "⏳ Processing"}
            {note.status === "Completed" && "✅ Completed"}
            {note.status === "Failed" && "❌ Failed"}
            </div>
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
          </>
        )}
      </div>
    );
  }