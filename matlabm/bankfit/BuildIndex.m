function index = BuildIndex(x, step)

	[row,col] = size(x);
	if step>row
		%±¨´í
	end
	seg = round(row/step);
	index = zeros(seg,col);
	for c=1:col
		for s=1:seg
			idxs = (s-1)*step+1;
			idxe = s*step;
			if idxe>row
				idxe = row;
			end
			d = x(idxs:idxe,c);
			index(s,c) = sum(d);
		end
	end


end