function [II,III]=Kands(x,pp)
%Kennard-Stone分集程序；
%x：待分类的总集；
%pp：训练样本数占总集的百分数,即分集比例；
%II：主集的样品序号，个数占总个数的pp%；
%III:第二集的样品序号。




	[xr,xc]=size(x);
	p=round(xc*pp/100);

	%The principal component analysis
	[U,S,V]=svd(x,0);
	B=diag(S);
	SB=sum(B);
	n=find((B./SB)>0.005);
	G=(V(:,n))';

	%The distance of euclid
	MM=pdist(G','euclid');
	MM=squareform(MM);

	%Find the first two members
	M=triu(MM);
	[maxm,I]=max(M);
	[maxx,i]=max(maxm);
	j=I(i);
	train=[x(:,i) x(:,j)];
	II=[i j];

	%Find other menbers

	for tr=3:p
	   M=MM(II,:);
	   M(:,II)=NaN;
	   [maxm,I]=min(M);
	   [maxx,i]=max(maxm);
	   train=[train x(:,i)];
	   II=[II i];
	end

	II=sort(II);
	III=1:xc;
	III(II)=[];

end