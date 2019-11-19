function search(index, d, step, delta)
    [seg,col] = size(index);
    for c=1:col
        temp = zeros(seg,1);
        for s=1:seg
            idxs = (s-1)*step+1;
			idxe = s*step;
			if idxe>length(d)
				idxe = length(d);
            end
            sub = d(idxs:idxe);
            temp(s) = abs(sum(sub)-index(s,c))<delta;
        end
        if sum(temp)==seg
            c
        end
    end
end