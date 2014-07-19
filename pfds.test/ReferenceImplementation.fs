﻿// ----------------------------------------------------------------------------------------------
// Copyright (c) Mårten Rånge.
// ----------------------------------------------------------------------------------------------
// This source code is subject to terms and conditions of the Microsoft Public License. A
// copy of the license can be found in the License.html file at the root of this distribution.
// If you cannot locate the  Microsoft Public License, please send an email to
// dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound
//  by the terms of the Microsoft Public License.
// ----------------------------------------------------------------------------------------------
// You must not remove this notice, or any other, from this software.
// ----------------------------------------------------------------------------------------------

namespace pfds.test

open pfds

module ReferenceImplementation =


    module RAList =

        type RAList<'T> = 'T list

        module Details =
            let rec lookupImpl (ri : int) (i : int) (ral : RAList<'T>) : 'T =
                match i, ral with
                | _, []     -> raise <| OutOfBoundsException ri
                | 0, x::_   -> x
                | i, _::xs  -> lookupImpl ri (i - 1) xs

            let rec updateImpl (ri : int) (i : int) (v : 'T) (ral : RAList<'T>) : RAList<'T> =
                match i, ral with
                | _, []     -> raise <| OutOfBoundsException ri
                | 0, _::xs  -> v::xs
                | i, x::xs  -> x::updateImpl ri (i - 1) v xs

        open Details

        let empty : RAList<'T>  = []

        let cons (v : 'T) (ral : RAList<'T>) : RAList<'T> = v::ral
        let uncons (ral : RAList<'T>) : 'T*RAList<'T> =
            match ral with
            | []    -> raise EmptyException
            | x::xs -> x,xs

        let head (ral : RAList<'T>)             =
            let h,_ = uncons ral
            h

        let tail (ral : RAList<'T>)             =
            let _,t = uncons ral
            t

        let inline lookup i ral                 = lookupImpl i i ral

        let inline update i v ral               = updateImpl i i v ral

        let fromSeq (s : seq<'T>) : RAList<'T>  = s |> Seq.toList

        let toList (ral : RAList<'T>)           = ral

        let toArray (ral : RAList<'T>)          = ral |> List.toArray


    module Queue =

        type Queue<'T> = 'T list*'T list

        let empty : Queue<'T> = [], []

        let snoc (v : 'T) ((fs, rs) : Queue<'T>) : Queue<'T> = fs, v::rs

        let uncons (q : Queue<'T>) : 'T*Queue<'T> =
            match q with
            | [], []    -> raise EmptyException
            | f::fs, rs -> f, (fs, rs)
            | _, rs     ->
                let fs = rs |> List.rev
                fs.Head, (fs.Tail, [])

        let head (q : Queue<'T>) : 'T           =
            let h, _ = uncons q
            h

        let tail (q : Queue<'T>) : Queue<'T>    =
            let _, t = uncons q
            t

        let fromSeq (s : seq<'T>) : Queue<'T>   = s |> Seq.toList,[]

        let toList ((fs, rs) : Queue<'T>)       = List.append fs (rs |> List.rev)

        let toArray (q : Queue<'T>)             = q |> toList |> List.toArray
