function [p,w,t] = IdentifyTrain(x,rank)

	[p, w, t, l] = ripppca(x, rank);

end