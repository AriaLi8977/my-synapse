import { useState, useEffect } from "react";
import { useParams, Link } from "react-router-dom";
import { getNoteById, deleteNote } from "../api/notesApi";
import type { Note } from "../types/note";
import { useNavigate } from "react-router-dom";


export function NoteDetailPage() {
    const { id } = useParams();
    const [note,setNote] = useState<Note | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(""); 

    const navigate = useNavigate();

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

    const statusInfo = statusMap[note.status] || {text: "Unknown", color: "bg-gray-100 text-gray-800"};

    useEffect(()=>{
        async function loadNote(){
            try{
                setLoading(true);
                const data = await getNoteById(id!);
                setNote(data);
            }catch(error){
                setError("Failed to load note details.");
            }finally{
                setLoading(false);
            }
        }
        loadNote();
    },[])
    if (loading) {
        return (
          <div className="p-8">
            Loading note...
          </div>
        );
      }
    
      if (error) {
        return (
          <div className="p-8 text-red-500">
            {error}
          </div>
        );
      }
    
      if (!note) {
        return (
          <div className="p-8">
            Note not found
          </div>
        );
      }

      const handleDelete = async () => {
        if (!note) return;
    
        const confirmed = window.confirm(
            "Are you sure you want to delete this note?"
        );
    
        if (!confirmed) return;
    
        try {
            await deleteNote(note.id);
            navigate("/");
        } catch {
            alert("Failed to delete note.");
        }
    };

    return (
        <div className="min-h-screen bg-gray-100">
            <div className="max-w-5xl mx-auto px-6 py-8">
    
                <div className="
                    flex items-center justify-between
                    border-b border-gray-200
                    pb-5 mb-6
                ">
    
                    {/* Left - Back Button */}
                    <div className="w-24">
                        <Link
                            to="/"
                            className="
                                inline-flex items-center gap-2
                                rounded-lg
                                border border-gray-200
                                bg-white
                                px-3 py-2
                                text-sm text-gray-700
                                hover:bg-gray-50
                                transition
                            "
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
                                    d="M15 19l-7-7 7-7"
                                />
                            </svg>
    
                            <span>Back</span>
                        </Link>
                    </div>
    
                    {/* Middle - Page Title */}
                    <h1 className="
                        text-xl font-semibold
                        text-gray-900
                    ">
                        Note Detail
                    </h1>
    
                    {/* Right - Delete Button */}
                    <div className="w-24 flex justify-end">
                        <button
                            onClick={handleDelete}
                            className="
                                inline-flex items-center justify-center
                                rounded-lg
                                border border-red-200
                                bg-white
                                p-2
                                text-red-500
                                hover:bg-red-50
                                transition
                            "
                            aria-label="Delete note"
                        >
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                className="h-5 w-5"
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
                </div>
    
                <div className="
                    bg-white
                    border border-gray-200
                    rounded-lg
                    p-8
                ">
    
                    <div className="border-b border-gray-200 pb-6">
    
                        <h2 className="
                            text-3xl font-semibold
                            text-gray-900
                            break-words
                        ">
                            {note.title || "Untitled Note"}
                        </h2>
    
                        <div className="
                            mt-4
                            flex items-center gap-3
                            text-sm text-gray-500
                        ">
                            <span>
                                Created{" "}
                                {new Date(note.createdAt).toLocaleString()}
                            </span>
    
                            <span className={`
                                inline-flex items-center
                                rounded-full
                                px-3 py-1
                                text-xs font-medium
                                ${statusInfo.color}
                            `}>
                                {statusInfo.text}
                            </span>
                        </div>
                    </div>
    
                    <div className="mt-8">
    
                        <h2 className="
                            text-sm font-semibold
                            text-gray-500 uppercase
                            tracking-wide mb-4
                        ">
                            Content
                        </h2>
    
                        <div className="
                            text-gray-800
                            whitespace-pre-wrap
                            leading-7
                        ">
                            {note.content}
                        </div>
                    </div>
    
                    {note.summary && (
                        <div className="mt-10">
    
                            <h2 className="
                                text-sm font-semibold
                                text-gray-500 uppercase
                                tracking-wide mb-4
                            ">
                                AI Summary
                            </h2>
    
                            <div className="
                                rounded-lg
                                border border-gray-200
                                bg-gray-50
                                p-6
                                text-gray-700
                                whitespace-pre-wrap
                                leading-7
                            ">
                                {note.summary}
                            </div>
                        </div>
                    )}
    
                </div>
            </div>
        </div>
    );
}